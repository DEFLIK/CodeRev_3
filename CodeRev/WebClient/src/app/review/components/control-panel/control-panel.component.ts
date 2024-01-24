import { Component, ElementRef, OnInit, ViewChild, ViewChildren, ViewContainerRef } from '@angular/core';
import { ControlPanelPage } from '../../models/controlPanelPage';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ReviewService } from '../../services/review.service';
import { User } from '../../models/response/user-response';
import { UserRole } from 'src/app/auth/models/userRole';
import { InterviewInfo } from '../../models/interviewInfo';
import { ProgrammingLanguage, convertProgrammingLanguageToString } from '../../models/programmingLanguage';
import { InterviewInfoResponse } from '../../models/response/interviewInfo-response';
import { TaskSelectionComponent } from '../task-selection/task-selection.component';

@Component({
    selector: 'app-control-panel',
    templateUrl: './control-panel.component.html',
    styleUrls: ['./control-panel.component.less']
})
export class ControlPanelComponent implements OnInit {

    @ViewChild('taskSelection')
    public set taskSelectionComponent(cmp : TaskSelectionComponent) {
        if (cmp) {
            this.taskSelectionCmpPlaceholder = cmp;
            setTimeout(() => cmp.setSelectedTasks(this.currentInterview!.tasks ?? []));
        }
    };
    public taskSelectionCmpPlaceholder!: TaskSelectionComponent;

    public allUsers?: User[];
    public allInterviews?: InterviewInfo[];
    public controlPanelPage: ControlPanelPage = ControlPanelPage.users;
    public searchForm: FormGroup = new FormGroup({
        searchInput: new FormControl('')
    });
    public currentUser?: User;
    public currentUserPrepare?: User;


    public currentInterview?: InterviewInfo;
    public currentInterviewPrepare?: InterviewInfo;


    public isEditing: boolean = false;
    public isDeleting: boolean = false;

    constructor(
        private _router: Router,
        private _reviewService: ReviewService,
    ) { }
    public ngOnInit(): void {
        this._reviewService
            .getAllUsers()
            .subscribe(resp => {
                if (resp.ok) {
                    this.allUsers = resp.body ?? [];
                }
            });
        this._reviewService
            .getInterviews()
            .subscribe(resp => {
                if (resp.ok) {
                    this.allInterviews = resp.body?.map(int => new InterviewInfo(int)) ?? [];
                }
            });
    }

    public selectControlPanelPage(page: ControlPanelPage): void {
        this.controlPanelPage = page;
    }

    public createInterview(): void {
        this._router.navigateByUrl(`review/create/interview`);
    }

    public createTask(): void {
        this._router.navigateByUrl(`review/create/task`);
    }

    public getRoleString(userRole: UserRole): string {
        switch(userRole) {
            case(3):
                return 'Администратор';
            case(0):
                return 'Кандидат';
            case(2):
                return 'HR менеджер';
            case(1):
                return 'Проверяющий';
            default:
                return 'Без роли';
        }
    }

    public openUserEditPanel(user: User): void {
        this.currentUserPrepare = new User(user.fullName, user.role, user.phoneNumber, user.email);
        this.currentUser = user;
        this.isEditing = true;
    }

    public closeEditPanel(): void {
        this.isEditing = false;
    }

    public editUser(): void {
        this._reviewService
            .editUser(this.currentUserPrepare!.email, this.currentUserPrepare!.fullName, this.currentUserPrepare!.role)
            .subscribe(res => {});
        this.currentUser!.fullName = this.currentUserPrepare!.fullName;
        this.currentUser!.role = this.currentUserPrepare!.role;
    }

    public openUserDeletePanel(user: User): void {
        this.isDeleting = true;
        this.currentUser = user;
    }

    public closeDeletePanel(): void {
        this.isDeleting = false;
    }

    public deleteUser(): void {
        this._reviewService
            .deleteUser(this.currentUser!)
            .subscribe(res => {});
    }

    public convertProgrammingLanguageToString(language: ProgrammingLanguage): string {
        return convertProgrammingLanguageToString(language);
    }

    public openInterviewEditPanel(interview: InterviewInfo): void {
        this.currentInterview = interview;
        this.currentInterviewPrepare = new InterviewInfo(new InterviewInfoResponse());
        this.currentInterviewPrepare.id = interview.id;
        this.currentInterviewPrepare.interviewText = interview.interviewText;
        this.currentInterviewPrepare.vacancy = interview.vacancy;
        this.isEditing = true;
    }

    public openInterviewDeletePanel(interview: InterviewInfo): void {
        this.isDeleting = true;
        this.currentInterview = interview;
    }

    public editInterview(): void {
        this.currentInterviewPrepare!.tasks = this.taskSelectionCmpPlaceholder.getSelectedTasks();

        this._reviewService
            .editInterview(this.currentInterviewPrepare!.id, this.currentInterviewPrepare!)
            .subscribe(resp => {
                if (resp.ok)
                {
                    this.currentInterview!.vacancy = this.currentInterviewPrepare!.vacancy;
                    this.currentInterview!.interviewText = this.currentInterviewPrepare!.interviewText;
                }
            });
    }

    public deleteInterview(): void {

    }
}
