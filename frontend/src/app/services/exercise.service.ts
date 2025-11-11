import { Injectable } from '@angular/core';
import { GenericRepositoryService } from '../services/generic-repository.service';
import { Observable, map } from 'rxjs';

export interface Exercise {
  id?: number;
  name: string;
  difficulty?: number;
  difficultyName?: string;
  bpm?: number;
  tabNotation?: string;
  tuningId?: number;
  tuning?: any;
  asset?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ExerciseService {
  private readonly endpoint = 'exercise';

  constructor(private repo: GenericRepositoryService<Exercise>) {}

  getAll(): Observable<Exercise[]> {
    return this.repo.getAll(this.endpoint).pipe(
      map((res: any) => {
        // Si la API devuelve el wrapper con data, lo usamos.
        if (res && Array.isArray(res.data)) {
          return res.data;
        }

        // Si devuelve el array directamente, también funciona.
        if (Array.isArray(res)) {
          return res;
        }

        // Si no hay data, devolvemos array vacío.
        return [];
      })
    );
  }

getById(id: number): Observable<Exercise | null> {
  return this.repo.getById(this.endpoint, id).pipe(
    map((res: any) => {
      // Si viene con wrapper
      if (res && res.data) return res.data;
      // Si viene el objeto directo
      if (res && res.id) return res;
      // Si viene vacío
      return null;
    })
  );
}


  create(entity: Omit<Exercise, 'id'>): Observable<Exercise | null> {
    return this.repo.create(this.endpoint, entity).pipe(
      map((res: any) => res?.data ?? null)
    );
  }

  update(id: number, entity: Exercise): Observable<Exercise | null> {
    return this.repo.update(this.endpoint, id, entity).pipe(
      map((res: any) => res?.data ?? null)
    );
  }

  delete(id: number): Observable<void> {
    return this.repo.delete(this.endpoint, id).pipe(map(() => void 0));
  }
}
