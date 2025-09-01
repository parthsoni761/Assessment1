import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
// --- FIX: Add HTTP_INTERCEPTORS to this import line ---
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module'; // Ensure filename is correct
import { App } from './app';
import { LoginComponent } from './auth/login/login';
import { SignupComponent } from './auth/signup/signup';
import { DashboardComponent } from './dashboard/dashboard'; // Ensure filename is correct
import { AuthInterceptor } from './interceptors/auth-interceptor';
import { WatchlistComponent } from './watchlist/watchlist'; // Ensure filename is correct

@NgModule({
  declarations: [
    App,
 // App is NOT standalone, so it stays here
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    AppRoutingModule,
    LoginComponent,
    SignupComponent,
    DashboardComponent,
    WatchlistComponent
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [App]
})
export class AppModule { }
