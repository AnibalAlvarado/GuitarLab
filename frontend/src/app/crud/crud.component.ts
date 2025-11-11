import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { GenericRepositoryService } from '../services/generic-repository.service';
import { ENTITY_SCHEMAS } from './crud-schemas';
import Swal from 'sweetalert2';
export interface CrudField {
  name: string;
  label: string;
  type: 'text' | 'number' | 'email' | 'password' | 'textarea' | 'select';
  required?: boolean;
  options?: { value: any; label: string }[];
  sourceEntity?: string; // 👈 para llaves foráneas dinámicas
}

export interface CrudSchema {
  displayName: string;
  fields: CrudField[];
  useJoinEndpoint?: boolean; // 👈 NUEVO: usar /join en lugar de /select
}

@Component({
  selector: 'app-crud',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './crud.component.html',
  styleUrls: ['./crud.component.css'],
})
export class CrudComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private repo = inject(GenericRepositoryService<any>);
  private snack = inject(MatSnackBar);

  entity = '';
  schema?: CrudSchema;
  form!: FormGroup;
  dataSource = new MatTableDataSource<any>([]);
  displayedColumns: string[] = [];
  isEditing = false;
  selectedItem: any | null = null;
  loading = false;
  filterValue = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.entity = this.route.snapshot.data['entity'];
    this.schema = ENTITY_SCHEMAS[this.entity];

    if (!this.schema) {
      this.showToast(`No schema found for entity "${this.entity}"`, 'error');
      return;
    }

    this.buildForm();
    this.loadItems();
    this.loadSelectOptions();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  private buildForm() {
    const controls: any = {};
    for (const field of this.schema!.fields) {
      controls[field.name] = [null, field.required ? Validators.required : []];
    }
    this.form = this.fb.group(controls);
  }

  private loadItems() {
    this.loading = true;

    const useJoin = this.schema?.useJoinEndpoint ?? false;

    const source$ = useJoin
      ? this.repo.getAllJoin(this.entity)
      : this.repo.getAll(this.entity);

    source$.subscribe({
      next: (res: any) => {
        const data: any[] = Array.isArray(res) ? res : (res?.data ?? []);
        this.dataSource.data = data;
        this.setupDisplayedColumns(data);
        this.loading = false;
      },
      error: (err: any) => {
        this.loading = false;
        this.showToast(`Error loading ${this.entity}: ${err.message}`, 'error');
      },
    });
  }




  private setupDisplayedColumns(data: any[]) {
    if (!data || !data.length) return;

    const sample = data[0];
    const allKeys = Object.keys(sample);
    const ignored = ['id', 'asset', 'createAt', 'updateAt', 'deleteAt'];

    // 🔹 Preferimos mostrar los campos definidos en el schema (en orden)
    const schemaFields = this.schema?.fields.map(f => f.name) ?? [];

    const visible: string[] = [];

    for (const field of schemaFields) {
      // 1️⃣ Si hay un campo "fieldName", usarlo
      const altName = `${field}Name`;
      if (allKeys.includes(altName)) {
        visible.push(altName);
        continue;
      }

      // 2️⃣ Si el campo apunta a relación y existe la entidad expandida
      if (allKeys.includes(field.replace('Id', '')) && field.endsWith('Id')) {
        visible.push(field.replace('Id', ''));
        continue;
      }

      // 3️⃣ Si el campo existe como tal, mostrarlo
      if (allKeys.includes(field)) visible.push(field);
    }

    this.displayedColumns = [...visible, 'actions'];
  }


  getDisplayValue(item: any, column: string): any {
    const value = item[column];

    // Si el valor es objeto (relación expandida), muestra su nombre legible
    if (value && typeof value === 'object') {
      return value.name || value.username || value.label || JSON.stringify(value);
    }

    // Si el valor es null o undefined
    if (value === null || value === undefined) return '-';

    return value;
  }


  private loadSelectOptions() {
    if (!this.schema) return;

    for (const field of this.schema.fields) {
      if (
        field.type === 'select' &&
        field.sourceEntity &&
        (!field.options || !field.options.length)
      ) {
        this.repo.getAll(field.sourceEntity).subscribe({
          next: (data) => {
            field.options = (data ?? []).map((x: any) => ({
              value: x.id,
              label: x.name || x.username || x.description || `Item ${x.id}`,
            }));
          },
          error: (err) => {
            console.error(`Error loading options for ${field.sourceEntity}:`, err);
          },
        });
      }
    }
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.filterValue = value.trim().toLowerCase();
    this.dataSource.filter = this.filterValue;
  }

  onSubmit() {
    if (this.form.invalid) {
      this.showToast('Please fill all required fields', 'warn');
      return;
    }

    const value = this.form.value;
    this.loading = true;

    if (this.isEditing && this.selectedItem?.id) {
      this.repo.update(this.entity, this.selectedItem.id, value).subscribe({
        next: () => {
          this.showToast(`${this.schema!.displayName} updated successfully`, 'success');
          this.reload();
        },
        error: (err) => {
          this.loading = false;
          this.showToast(`Error updating record: ${err.message}`, 'error');
        },
      });
    } else {
      this.repo.create(this.entity, value).subscribe({
        next: () => {
          this.showToast(`${this.schema!.displayName} created successfully`, 'success');
          this.reload();
        },
        error: (err) => {
          this.loading = false;
          this.showToast(`Error creating record: ${err.message}`, 'error');
        },
      });
    }
  }

  editItem(item: any) {
    this.isEditing = true;
    this.selectedItem = item;
    this.form.patchValue(item);
    this.showToast('Editing mode enabled', 'info');
  }

  async deleteItem(id: number) {
    const result = await Swal.fire({
      title: 'Are you sure?',
      text: 'This record will be permanently deleted.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Yes, delete it',
      cancelButtonText: 'Cancel',
      background: '#1a0b2e',
      color: '#fff',
    });

    if (result.isConfirmed) {
      this.loading = true;

      this.repo.delete(this.entity, id).subscribe({
        next: () => {
          Swal.fire({
            icon: 'success',
            title: 'Deleted!',
            text: 'The record has been removed successfully.',
            timer: 1800,
            showConfirmButton: false,
            background: '#1a0b2e',
            color: '#fff',
          });
          this.loadItems();
        },
        error: (err) => {
          this.loading = false;
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: `Failed to delete record: ${err.message}`,
            background: '#1a0b2e',
            color: '#fff',
          });
        },
      });
    }
  }


  resetForm() {
    this.isEditing = false;
    this.selectedItem = null;
    this.form.reset();
  }

  reload() {
    this.resetForm();
    this.loadItems();
  }

  goBack() {
    this.router.navigate(['/dashboard']);
  }

  private showToast(
    message: string,
    type: 'success' | 'error' | 'warn' | 'info' = 'info'
  ) {
    const panelClass = {
      success: 'toast-success',
      error: 'toast-error',
      warn: 'toast-warn',
      info: 'toast-info',
    }[type];

    this.snack.open(message, 'OK', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: [panelClass],
    });
  }
}
