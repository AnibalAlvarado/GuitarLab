// src/app/services/generic-repository.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface BaseEntity {
  id?: number;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message: string;
  details?: any;
}

@Injectable({
  providedIn: 'root'
})
export class GenericRepositoryService<T extends BaseEntity> {
  private readonly API_URL = environment.API_URL;

  constructor(private http: HttpClient) { }

  /**
   * Obtener todos los registros
   * El backend devuelve ApiResponse<T[]>, así que desempaquetamos .data
   */
  getAll(endpoint: string): Observable<T[]> {
    return this.http.get<ApiResponse<T[]>>(`${this.API_URL}${endpoint}/select`, { withCredentials: true }).pipe(
      map(res => res?.data ?? []),
      catchError(err => {
        console.error(`[GET ALL] Error fetching ${endpoint}:`, err);
        return of([]);
      })
    );
  }

  /**
   * Obtener un registro por ID
   */
  getById(endpoint: string, id: number): Observable<T | null> {
    return this.http.get<ApiResponse<T>>(`${this.API_URL}${endpoint}/${id}`, { withCredentials: true }).pipe(
      map(res => res?.data ?? null),
      catchError(err => {
        console.error(`[GET BY ID] Error fetching ${endpoint}/${id}:`, err);
        return of(null);
      })
    );
  }

  /**
   * Crear un nuevo registro
   */
  create(endpoint: string, entity: Omit<T, 'id'>): Observable<T | null> {
    return this.http.post<ApiResponse<T>>(`${this.API_URL}${endpoint}`, entity, { withCredentials: true }).pipe(
      map(res => res?.data ?? null),
      catchError(err => {
        console.error(`[CREATE] Error creating ${endpoint}:`, err);
        return of(null);
      })
    );
  }

  /**
   * Actualizar un registro existente
   */
  update(endpoint: string, id: number, entity: T): Observable<T | null> {
    return this.http.put<ApiResponse<T>>(`${this.API_URL}${endpoint}/${id}`, entity, { withCredentials: true }).pipe(
      map(res => res?.data ?? null),
      catchError(err => {
        console.error(`[UPDATE] Error updating ${endpoint}/${id}:`, err);
        return of(null);
      })
    );
  }

  /**
   * Eliminado lógico (soft delete)
   */
  delete(endpoint: string, id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<any>>(`${this.API_URL}${endpoint}/${id}`, { withCredentials: true }).pipe(
      map(res => res?.success ?? false),
      catchError(err => {
        console.error(`[DELETE] Error deleting ${endpoint}/${id}:`, err);
        return of(false);
      })
    );
  }

  /**
   * Eliminado permanente
   */
  permanentDelete(endpoint: string, id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<any>>(`${this.API_URL}${endpoint}/permanent/${id}`, { withCredentials: true }).pipe(
      map(res => res?.success ?? false),
      catchError(err => {
        console.error(`[PERMANENT DELETE] Error deleting ${endpoint}/permanent/${id}:`, err);
        return of(false);
      })
    );
  }

  /**
   * Endpoint dinámico para consultas personalizadas (e.g. /dynamic)
   */
  getDynamic(endpoint: string): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.API_URL}${endpoint}/dynamic`, { withCredentials: true }).pipe(
      map(res => res?.data ?? null),
      catchError(err => {
        console.error(`[DYNAMIC] Error fetching ${endpoint}/dynamic:`, err);
        return of(null);
      })
    );
  }

  getAllJoin(endpoint: string): Observable<T[]> {
  return this.http.get<ApiResponse<T[]>>(`${this.API_URL}${endpoint}/join`, { withCredentials: true }).pipe(
    map(res => res?.data ?? []),
    catchError(err => {
      console.error(`[GET JOIN] Error fetching ${endpoint}/join:`, err);
      return of([]);
    })
  );
}

}
