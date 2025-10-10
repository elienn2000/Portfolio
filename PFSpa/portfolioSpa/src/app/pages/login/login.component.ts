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

import { User } from '../../models/user.model'

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
    }, { validator: this.passwordMatchValidator });
    
    
  }
  
  onLogin() {
    if (this.loginForm.valid) {

    const loginValue = this.loginForm.get('username')?.value;
    const password = this.loginForm.get('password')?.value;
        
    let email: string | null = null;
    let username: string | null = null;
        
    if (loginValue?.includes('@')) {
      email = loginValue;
    } else {
      username = loginValue;
    }
    
    var payload = {
      email: email || '',
      username: username || '',
      password: password
    };


      // Per ora solo console.log, più avanti gestirai i token
      this.authService.login(payload).subscribe({
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


      var email = this.registerForm.get('email')?.value;
      var username = this.registerForm.get('username')?.value;
      var password = this.registerForm.get('password')?.value;
      // Per ora solo console.log, più avanti gestirai i token
      this.authService.register({ email, username, password  }).subscribe({
        next: (response) => {
          console.log('Login successful:', response);
        },
        error: (error) => {
          console.error('Login failed:', error);
        }
      });
    
  }
  
  
  passwordMatchValidator(formGroup: FormGroup) {
  const password = formGroup.get('password')?.value;
  const confirmPassword = formGroup.get('confirmPassword')?.value;

  if (password !== confirmPassword) {
    formGroup.get('confirmPassword')?.setErrors({ mismatch: true });
  } else {
    formGroup.get('confirmPassword')?.setErrors(null);
  }

  return null;
}
}
