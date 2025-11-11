import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ExerciseService, Exercise } from '../../services/exercise.service';
import { GenericRepositoryService } from '../../services/generic-repository.service';
import { TabEditorComponent } from '../tab-editor/tab-editor.component';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-exercise-editor',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatSnackBarModule,
    TabEditorComponent,
    MatIconModule
  ],
  templateUrl: './exercise-editor.component.html',
  styleUrls: ['./exercise-editor.component.css']
})
export class ExerciseEditorComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snack = inject(MatSnackBar);
  private service = inject(ExerciseService);
  private repo = inject(GenericRepositoryService<any>);

  form!: FormGroup;
  id?: number;
  tunings: any[] = [];
  tabData = '';

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id'));
    this.buildForm();
    this.loadTunings();

    if (this.id) {
      this.service.getById(this.id).subscribe((data) => {
        // ✅ Verificamos que no sea null antes de usar
        if (data) {
          this.form.patchValue({
            name: data.name,
            bpm: data.bpm,
            tuningId: data.tuningId,
            difficulty: data.difficulty, // ahora numérico
            tabNotation: data.tabNotation
          });
          this.tabData = data.tabNotation || '';
        }
        else {
          this.snack.open('Exercise not found', 'OK', { duration: 2500 });
          this.router.navigate(['/exercises']);
        }
      });
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      difficulty: ['', Validators.required],
      bpm: [120, Validators.required],
      tuningId: [null, Validators.required],
      tabNotation: ['']
    });
  }

  loadTunings() {
    this.repo.getAll('tuning').subscribe((data) => (this.tunings = data ?? []));
  }

  save() {
    if (this.form.invalid) {
      this.snack.open('Please complete all fields', 'OK', { duration: 2500 });
      return;
    }

    const payload: Exercise = {
      ...this.form.value,
      tabNotation: this.tabData
    };

    const request = this.id
      ? this.service.update(this.id, payload)
      : this.service.create(payload);

    request.subscribe({
      next: () => {
        this.snack.open('Exercise saved successfully', 'OK', { duration: 2500 });
        this.router.navigate(['/exercises']);
      },
      error: (err) => this.snack.open(`Error: ${err.message}`, 'OK', { duration: 2500 })
    });
  }

  cancel() {
    this.router.navigate(['/exercises']);
  }
}
