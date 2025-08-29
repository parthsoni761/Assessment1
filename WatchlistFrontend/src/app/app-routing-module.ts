import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Make sure these paths and component names are correct
import { LoginComponent } from './auth/login/login';
import { SignupComponent } from './auth/signup/signup';
import { DashboardComponent } from './dashboard/dashboard';

// This is the routes array you were trying to import directly
const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule] // This exports RouterModule so <router-outlet> works
})
export class AppRoutingModule { }
