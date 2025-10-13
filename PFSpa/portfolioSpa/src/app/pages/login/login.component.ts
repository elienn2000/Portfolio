import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule } from '@angular/forms';

import { AuthService } from '../../services/auth.service';

import { User } from '../../models/user.model';

import {MatDialog} from '@angular/material/dialog';
import { EmailConfirmDialog } from './dialog-confirm-mail/dialog-confirm-mail';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, MatCardModule, MatTabsModule, MatInputModule, MatFormFieldModule, ReactiveFormsModule, MatButtonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  loginForm: FormGroup;
  registerForm: FormGroup;

  newUser: User | null = null

  verificationCode = '';

  readonly dialog = inject(MatDialog);

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

    this.openConfimMailDialog();

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

    // Checking if the form is valid
    if (this.registerForm.invalid)
      return;

    // Extracting form values
    var email = this.registerForm.get('email')?.value;
    var username = this.registerForm.get('username')?.value;
    var password = this.registerForm.get('password')?.value;

    // Preparing payload
    // The backend expects email, username, and password for the user registration
    this.authService.register({ email, username, password }).subscribe({
      next: (response) => {
        // Storing the new user data for preparing the login
        this.newUser = response;

        // Preparing the email verification step
        this.onHandleVerificationMail();

        // Resetting the registration form
        //this.registerForm.reset();

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

  // Call this method for showing the email verification message and sending the email
  onHandleVerificationMail() {

    if (!this.newUser || !this.newUser.email) {
      return;
      // Safety check: if there's no new user or email, do nothing TODO: show error
    }

    this.authService.sendVerificationEmail(this.newUser?.email).subscribe({
      next: () => {
        console.log('Verification email sent successfully');
        this.openConfimMailDialog();
      },
      error: (error) => {
        console.error('Failed to send verification email:', error);
      }
    });

  }


  openConfimMailDialog(): void {
    // Controlla se EmailConfirmDialog è già aperto
    const isOpen = this.dialog.openDialogs.some(
      dialog => dialog.componentInstance instanceof EmailConfirmDialog
    );
    
    if (isOpen) {
      console.log('EmailConfirmDialog already open');
      return;
    }

    const dialogRef = this.dialog.open(EmailConfirmDialog, {
      // data: {name: this.name(), animal: this.animal()},
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      if (result !== undefined) {
        //this.animal.set(result);
      }
    });
  }
}

