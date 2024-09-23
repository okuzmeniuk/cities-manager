import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { LoginUser } from '../models/login-user';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoginFormSubmitted = false;

  constructor(private accountService: AccountService, private router: Router) {
    this.loginForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required]),
    });
  }

  get login_emailControl() {
    return this.loginForm.controls['email'];
  }

  get login_passwordControl() {
    return this.loginForm.controls['password'];
  }

  loginSubmitted() {
    this.isLoginFormSubmitted = true;

    if (!this.loginForm.valid) {
      return;
    }

    this.accountService.postLogin(this.loginForm.value).subscribe({
      next: (response: any) => {
        this.isLoginFormSubmitted = false;
        this.accountService.currentUserName = response.email;
        localStorage['token'] = response.token;
        localStorage['refreshToken'] = response.refreshToken;
        this.router.navigate(['/cities']);
        this.loginForm.reset();
      },

      error: (err) => console.log(err),
    });
  }
}
