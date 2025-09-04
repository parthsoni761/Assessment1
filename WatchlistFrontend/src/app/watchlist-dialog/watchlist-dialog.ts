import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../material.module';
import { WatchlistItemDto } from '../models/watchlist-item';

@Component({
  selector: 'app-watchlist-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './watchlist-dialog.html',
  styleUrls: ['./watchlist-dialog.css']
})
export class WatchlistDialogComponent implements OnInit {
  public itemForm: FormGroup;
  public isEditing: boolean;

  constructor(
    public dialogRef: MatDialogRef<WatchlistDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { item: WatchlistItemDto, isEditing: boolean },
    private fb: FormBuilder
  ) {
    this.isEditing = data.isEditing;
    const item = data.item;

    this.itemForm = this.fb.group({
      userId: [item.userId],
      title: [item.title, [Validators.required, Validators.maxLength(100)]],
      itemType: [item.itemType, Validators.required],
      status: [item.status, Validators.required],
      releaseYear: [item.releaseYear, [Validators.required, Validators.min(1888), Validators.max(new Date().getFullYear())]],
      genre: [item.genre, Validators.required],
      // --- FIX #2: Change rating validator to min(1) ---
      rating: [item.rating, [Validators.required, Validators.min(1), Validators.max(5)]],
      isFavorite: [item.isFavorite],
      completedEpisodes: [item.completedEpisodes, [Validators.min(0)]],
      totalEpisodes: [item.totalEpisodes, [Validators.min(0)]]
    }, {
      // --- FIX #1: Apply the custom validator to the entire form group ---
      validators: this.episodesValidator
    });
  }

  ngOnInit(): void {
    // This logic correctly shows/hides the episode fields based on itemType
    this.itemForm.get('itemType')?.valueChanges.subscribe(value => {
      const completedEpisodesControl = this.itemForm.get('completedEpisodes');
      const totalEpisodesControl = this.itemForm.get('totalEpisodes');
      if (value === 'TV Show') {
        completedEpisodesControl?.setValidators([Validators.min(0)]);
        totalEpisodesControl?.setValidators([Validators.min(0)]);
      } else {
        completedEpisodesControl?.clearValidators();
        totalEpisodesControl?.clearValidators();
      }
      completedEpisodesControl?.updateValueAndValidity();
      totalEpisodesControl?.updateValueAndValidity();
    });
  }

  // --- FIX #1: Custom validator function ---
  episodesValidator(group: AbstractControl): ValidationErrors | null {
    const total = group.get('totalEpisodes')?.value;
    const completed = group.get('completedEpisodes')?.value;
    const completedControl = group.get('completedEpisodes');

    // Only run validation if both values are present and total is greater than 0
    if (total > 0 && completed > total) {
      // Set the error on the specific control so the error message appears in the right place
      completedControl?.setErrors({ completedTooHigh: true });
      return { completedTooHigh: true }; // Return the error for the group as well
    }

    // If the error was previously set but is now valid, we need to clear it.
    if (completedControl?.hasError('completedTooHigh')) {
      // Create a copy of existing errors, remove our custom one, and set it back.
      const errors = { ...completedControl.errors };
      delete errors['completedTooHigh'];
      // Set errors to null if no other errors exist, otherwise just set the remaining ones.
      completedControl.setErrors(Object.keys(errors).length > 0 ? errors : null);
    }

    return null; // The form group is valid in this regard
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.itemForm.valid) {
      this.dialogRef.close(this.itemForm.value);
    }
  }
}
