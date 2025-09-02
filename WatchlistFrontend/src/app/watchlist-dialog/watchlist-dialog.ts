import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators,
  AbstractControl,
  ValidationErrors } from '@angular/forms';
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
      releaseYear: [item.releaseYear, [Validators.required, Validators.min(1888), Validators.max(2099)]],
      genre: [item.genre],
      rating: [item.rating, [Validators.required, Validators.min(0), Validators.max(5)]],
      isFavorite: [item.isFavorite],
      //completedEpisodes: [item.completedEpisodes],
      //totalEpisodes: [item.totalEpisodes],
      totalEpisodes: [data.item.totalEpisodes || 0, [Validators.required, Validators.min(0)]],
      completedEpisodes: [data.item.completedEpisodes || 0, [Validators.required, Validators.min(0)]],

    },
      { validators: this.completedNotGreaterValidator }
    );
  }
  completedNotGreaterValidator(group: AbstractControl): ValidationErrors | null {
    const total = group.get('totalEpisodes')?.value;
    const completed = group.get('completedEpisodes')?.value;
    if (completed != null && total != null && completed > total) {
      return { completedTooHigh: true };
    }
    return null;
  }

  ngOnInit(): void {
    // This logic does not need to change
    this.itemForm.get('itemType')?.valueChanges.subscribe(value => {
      const completedEpisodesControl = this.itemForm.get('completedEpisodes');
      const totalEpisodesControl = this.itemForm.get('totalEpisodes');
      if (value === 'TV Show') {
        completedEpisodesControl?.setValidators(Validators.min(0));
        totalEpisodesControl?.setValidators(Validators.min(0));
      } else {
        completedEpisodesControl?.clearValidators();
        totalEpisodesControl?.clearValidators();
      }
      completedEpisodesControl?.updateValueAndValidity();
      totalEpisodesControl?.updateValueAndValidity();
    });
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
