import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardContent, MatCardTitle} from '@angular/material/card';
import {MatError, MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {NgIf} from '@angular/common';
import {AuthService, UserRegistration} from '../../../shared/services/auth.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    MatButton,
    MatCard,
    MatCardContent,
    MatCardTitle,
    MatError,
    MatFormField,
    MatInput,
    MatLabel,
    NgIf,
    ReactiveFormsModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;

  constructor(private fb: FormBuilder,
              private authService: AuthService,
              private router: Router,
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigateByUrl('/dashboard');
    }
  }

  onSubmit() {
    if (this.registerForm.valid) {

      let firstNameCapitalized: string = this.registerForm.value['firstName'].toLowerCase();
      firstNameCapitalized = firstNameCapitalized.charAt(0).toUpperCase() + firstNameCapitalized.slice(1);

      let surnameCapitalized: string = this.registerForm.value['surname'].toLowerCase();
      surnameCapitalized = surnameCapitalized.charAt(0).toUpperCase() + surnameCapitalized.slice(1);

      const registrationObject: UserRegistration = {
        email: this.registerForm.value['email'],
        password: this.registerForm.value['password'],
        name: firstNameCapitalized,
        surname: surnameCapitalized
      }

      this.authService.registerNewUser(registrationObject).subscribe(
        {
          next: res => {
            this.router.navigateByUrl(`/login`)
          },
          error: err => {
            this.registerForm.reset()
          }
        }
      )
    }
  }
}
