import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { SignupComponent } from './auth/signup/signup';
import { LayoutComponent } from './layout/layout';
import { DashboardComponent } from './dashboard/dashboard';
import { WatchlistComponent } from './watchlist/watchlist';
import { authGuard } from './guards/auth-guard';

const routes: Routes = [
  // Public routes, accessible without logging in
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },

  // Protected routes, all nested under the main app layout
  {
    path: 'app', // All main app routes will be like /app/dashboard
    component: LayoutComponent,
    canActivate: [authGuard], // This guard protects all child routes
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'watchlist', component: WatchlistComponent },
      // Default route for a logged-in user
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // Fallback routes
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Default route for the whole app
  { path: '**', redirectTo: '/login' } // Wildcard route for any other path
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
