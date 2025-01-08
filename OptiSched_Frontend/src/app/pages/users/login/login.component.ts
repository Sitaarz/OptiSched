import {Component, Inject, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardTitle} from '@angular/material/card';
import {MatError, MatFormField, MatLabel} from '@angular/material/form-field';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatInput} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {DOCUMENT, NgIf} from '@angular/common';
import {AuthService} from '../../../shared/services/auth.service';
import {ActivatedRoute, Router} from '@angular/router';
import {TOKEN_KEY} from '../../../shared/constants/constants';


@Component({
  selector: 'app-login',
  imports: [
    MatError,
    MatLabel,
    MatFormField,
    MatCardContent,
    MatCard,
    MatInput,
    ReactiveFormsModule,
    MatButton,
    NgIf,
    MatCardTitle
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  protected loginForm: FormGroup;
  private localStorage


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    @Inject(DOCUMENT) private document: Document) {
    this.localStorage = document.defaultView?.localStorage;
    this.loginForm = this.fb.group({
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
    if (this.loginForm.valid) {
      this.authService.loginUser(this.loginForm.value).subscribe(
        {
          next: res => {
            localStorage.setItem(TOKEN_KEY, res.toString())
            this.router.navigateByUrl('/dashboard')
          },
          error: err => {
            this.loginForm.controls['password'].reset()
            console.log(err)
          }
        }
      )
    }
  }
}
