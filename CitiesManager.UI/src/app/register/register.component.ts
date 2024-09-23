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
import { RegisterUser } from '../models/register-user';
import { CompareValidation } from '../validators/custom-validators';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  registerForm: FormGroup;
  isRegisterFormSubmitted = false;

  constructor(private accountService: AccountService, private router: Router) {
    this.registerForm = new FormGroup(
      {
        personName: new FormControl(null, [Validators.required]),
        email: new FormControl(null, [Validators.required, Validators.email]),
        phoneNumber: new FormControl(null, [Validators.required]),
        password: new FormControl(null, [Validators.required]),
        confirmPassword: new FormControl(null, [Validators.required]),
      },
      { validators: [CompareValidation('password', 'confirmPassword')] }
    );
  }

  get register_personNameControl() {
    return this.registerForm.controls['personName'];
  }

  get register_emailControl() {
    return this.registerForm.controls['email'];
  }

  get register_phoneNumberControl() {
    return this.registerForm.controls['phoneNumber'];
  }

  get register_passwordControl() {
    return this.registerForm.controls['password'];
  }

  get register_confirmPasswordControl() {
    return this.registerForm.controls['confirmPassword'];
  }

  registerSubmitted() {
    this.isRegisterFormSubmitted = true;

    if (!this.registerForm.valid) {
      return;
    }

    this.accountService.postRegister(this.registerForm.value).subscribe({
      next: (response: any) => {
        this.isRegisterFormSubmitted = false;
        localStorage['token'] = response.token;
        localStorage['refreshToken'] = response.refreshToken;
        this.router.navigate(['/cities']);
        this.registerForm.reset();
      },

      error: (err) => console.log(err),
    });
  }
}
