import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./login/login.component').then(m => m.LoginComponent) },
  { path: 'dashboard', loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard] },

  // 👇 Usa los mismos nombres del ENTITY_SCHEMAS
  { path: 'exercises', loadComponent: () => import('./exercises/exercise-list/exercise-list.component').then(m => m.ExerciseListComponent) },
  { path: 'exercises/new', loadComponent: () => import('./exercises/exercise-editor/exercise-editor.component').then(m => m.ExerciseEditorComponent) },
  { path: 'exercises/edit/:id', loadComponent: () => import('./exercises/exercise-editor/exercise-editor.component').then(m => m.ExerciseEditorComponent) },

  { path: 'guitarists', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'guitarist' } },
  { path: 'lessons', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'lesson' } },
  { path: 'techniques', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'technique' } },
  { path: 'tunings', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'tuning' } },
  { path: 'users', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'user' } },

  // 🔹 PIVOTES
  { path: 'guitarist-lessons', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'guitaristlesson' } },
  { path: 'lesson-exercises', loadComponent: () => import('./crud/crud.component').then(m => m.CrudComponent), canActivate: [authGuard], data: { entity: 'lessonexercise' } },
  { path: '**', redirectTo: '/login' }
];
