import {Routes} from '@angular/router';
import {HomeComponent} from './pages/home/home.component';
import {LoginComponent} from './pages/users/login/login.component';
import {RegisterComponent} from './pages/users/register/register.component';
import {DashboardComponent} from './pages/dashboard/dashboard.component';
import {authGuard} from './shared/auth.guard';

export const routes: Routes = [
  {
    path: '', component: HomeComponent,

  },
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {
    path: 'dashboard', component: DashboardComponent,
    canActivate: [authGuard],
  }
];
