import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { AuthInterceptor } from './interceptors/auth-interceptor';

// Import all standalone components
import { LoginComponent } from './auth/login/login';
import { SignupComponent } from './auth/signup/signup';
import { LayoutComponent } from './layout/layout';
import { NavbarComponent } from './navbar/navbar';
import { SidebarComponent } from './sidebar/sidebar';
import { DashboardComponent } from './dashboard/dashboard';
import { WatchlistComponent } from './watchlist/watchlist';
import { WatchlistDialogComponent } from './watchlist-dialog/watchlist-dialog';

@NgModule({
  declarations: [
    App // The only non-standalone component
  ],
  imports: [
    // Angular Core Modules
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,

    // App-specific Modules
    AppRoutingModule,
    MaterialModule,

    // All Standalone Components are imported like modules
    LoginComponent, SignupComponent, LayoutComponent, NavbarComponent,
    SidebarComponent, DashboardComponent, WatchlistComponent, WatchlistDialogComponent
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [App]
})
export class AppModule { }
