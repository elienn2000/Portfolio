import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
  { path: '', component: LoginComponent }, // default
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '' }
];
