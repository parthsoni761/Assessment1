import { NgModule } from '@angular/core';

// Form & Input Modules
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';

// Table & Sorting
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';

// Popups & Notifications
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';

// Icons & Indicators
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// Layout & Navigation
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';

@NgModule({
  exports: [
    MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatCheckboxModule,
    MatTableModule, MatSortModule, MatDialogModule, MatSnackBarModule, MatIconModule,
    MatProgressSpinnerModule, MatToolbarModule, MatSidenavModule, MatListModule, MatMenuModule,
    MatCardModule,
  ]
})
export class MaterialModule { }
