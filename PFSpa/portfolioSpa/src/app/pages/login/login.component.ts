import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { AuthService } from '../../services/auth.service';

import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, MatCardModule, MatTabsModule, MatInputModule, MatFormFieldModule, ReactiveFormsModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  // email = '';
  // password = '';

  loginForm: FormGroup;
  registerForm: FormGroup;

  constructor(private authService: AuthService, private fb: FormBuilder) {


    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    });


  }

  onLogin() {
    if (this.loginForm.valid) {
      // Per ora solo console.log, più avanti gestirai i token
      this.authService.login({ email: this.loginForm.get('email')?.value, password: this.loginForm.get('password')?.value }).subscribe({
        next: (response) => {
          console.log('Login successful:', response);
        },
        error: (error) => {
          console.error('Login failed:', error);
        }
      });
    }
  }

  onRegister() {
    if (this.registerForm.valid) {
      // Per ora solo console.log, più avanti gestirai i token
      this.authService.login({ email: this.registerForm.get('email')?.value, password: this.loginForm.get('password')?.value }).subscribe({
        next: (response) => {
          console.log('Login successful:', response);
        },
        error: (error) => {
          console.error('Login failed:', error);
        }
      });
    }
  }
}
