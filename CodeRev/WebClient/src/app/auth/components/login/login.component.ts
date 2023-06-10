import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service/auth.service';
import { VkSession } from '../../models/vkSession';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.less']
})
export class LoginComponent {
    public get isProcessing(): boolean {
        return this._auth.isProcessing;
    }
    public loginForm: FormGroup = new FormGroup({
        userName: new FormControl('', Validators.required),
        userPassword: new FormControl('', Validators.required)
    });

    constructor(private _auth: AuthService) {
    }

    public submit(): void {
        this._auth.login(
            this.loginForm.get('userName')?.value, 
            this.loginForm.get('userPassword')?.value)
            .subscribe();
    }

    public loginVK(): void {
        this._auth.getVkSession((vkAns: any) => {
            this._auth.loginViaVk(
                vkAns.session.user.id,
                new VkSession(
                    vkAns.session.expire,
                    vkAns.session.mid,
                    vkAns.session.secret,
                    vkAns.session.sid,
                    vkAns.session.sig
                )
            );
        });
    }
}