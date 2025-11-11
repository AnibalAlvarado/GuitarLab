import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ExerciseService, Exercise } from '../../services/exercise.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-exercise-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './exercise-list.component.html',
  styleUrls: ['./exercise-list.component.css']
})
export class ExerciseListComponent implements OnInit {
  private router = inject(Router);
  private service = inject(ExerciseService);

  displayedColumns = ['id', 'name', 'difficulty', 'bpm', 'actions'];
  items: Exercise[] = [];
  filteredItems: Exercise[] = [];
  loading = false;
  filterValue = '';

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.service.getAll().subscribe({
      next: (data) => {
        console.log('🔍 Exercises received:', data);
        this.items = data ?? [];
        this.filteredItems = this.items;
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Error loading exercises', err);
        this.loading = false;
      }
    });
  }

  applyFilter() {
    const value = this.filterValue.toLowerCase();
    this.filteredItems = this.items.filter(
      (x) =>
        x.name.toLowerCase().includes(value) ||
        (x.difficultyName ?? '').toLowerCase().includes(value) ||
        x.bpm?.toString().includes(value)
    );
  }

  editItem(id: number) {
    this.router.navigate(['/exercises/edit', id]);
  }

  newItem() {
    this.router.navigate(['/exercises/new']);
  }

  deleteItem(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'This exercise will be permanently deleted.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Yes, delete it',
      cancelButtonText: 'Cancel',
      background: '#1a0b2e',
      color: '#fff',
    }).then((result) => {
      if (result.isConfirmed) {
        this.service.delete(id).subscribe({
          next: () => {
            Swal.fire({
              icon: 'success',
              title: 'Deleted!',
              text: 'The exercise has been removed successfully.',
              timer: 1800,
              showConfirmButton: false,
              background: '#1a0b2e',
              color: '#fff',
            });
            this.loadData();
          },
          error: (err) => {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: `Failed to delete exercise: ${err.message}`,
              background: '#1a0b2e',
              color: '#fff',
            });
          },
        });
      }
    });
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }
}
