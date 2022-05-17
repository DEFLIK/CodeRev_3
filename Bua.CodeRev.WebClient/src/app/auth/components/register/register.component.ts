import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth-service/auth.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.less']
})
export class RegisterComponent implements OnInit {
    public get isProcessing(): boolean {
        return this._auth.isProcessing;
    }
    public regForm: FormGroup = new FormGroup({
        userName: new FormControl('', Validators.required),
        userEmail: new FormControl('', [
            Validators.required,
            Validators.pattern('[a-zA-Z_]+@[a-zA-Z_]+?.[a-zA-Z]{2,3}')
        ]),
        userPhone: new FormControl('', Validators.required),
        userPassword: new FormControl('', Validators.required),
    });

    public inviteToken: string = '';

    constructor(
        private _auth: AuthService,
        private _activatedRoute: ActivatedRoute
    ) {}
    public ngOnInit(): void {
        this.inviteToken = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';
    }

    public submit(): void {
        const name = this.regForm.get('userName')?.value;
        const email = this.regForm.get('userEmail')?.value;
        const phone = this.regForm.get('userPhone')?.value;
        const pass = this.regForm.get('userPassword')?.value;

        this._auth.register(
            this.inviteToken,
            name, 
            email,
            phone,
            pass)
            .subscribe(resp => {
                if (resp.ok) {
                    this._auth.login(
                        email,
                        pass);
                }
            });
    }
}
