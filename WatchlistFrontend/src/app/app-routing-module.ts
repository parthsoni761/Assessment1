import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { SignupComponent } from './auth/signup/signup';
import { LayoutComponent } from './layout/layout';
import { DashboardComponent } from './dashboard/dashboard';
import { WatchlistComponent } from './watchlist/watchlist'; // We will create this next
import { authGuard } from './guards/auth-guard'; // Import the guard

const routes: Routes = [
  // Routes that DON'T use the sidebar layout
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },

  // Route that uses the sidebar layout, protected by the authGuard
  {
    path: '', // This will be the default for logged-in users
    component: LayoutComponent,
    canActivate: [authGuard], // Protect this whole section
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'watchlist', component: WatchlistComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' } // Default child route
    ]
  },

  // Fallback for any other path, redirect to login
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
