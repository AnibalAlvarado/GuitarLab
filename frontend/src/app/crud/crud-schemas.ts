import { CrudSchema } from './crud.component';

/**
 * Configuración centralizada de esquemas para el CRUD dinámico.
 * Cada clave corresponde al nombre de la entidad usada en el backend.
 * 
 * 🔹 Si el campo tiene `options`, el select será estático.
 * 🔹 Si el campo tiene `sourceEntity`, el select se llenará automáticamente
 *     consultando ese endpoint (por ejemplo /api/technique/select).
 */
export const ENTITY_SCHEMAS: Record<string, CrudSchema> = {
  guitarist: {
    displayName: 'Guitarists',
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      {
        name: 'skillLevel',
        label: 'Skill Level',
        type: 'select',
        options: [
          { value: 'Beginner', label: 'Beginner' },
          { value: 'Intermediate', label: 'Intermediate' },
          { value: 'Advanced', label: 'Advanced' }
        ]
      },
      { name: 'experienceYears', label: 'Experience (Years)', type: 'number' }
    ]
  },

  lesson: {
    displayName: 'Lessons',
    useJoinEndpoint: true,
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' },
      {
        name: 'techniqueId',
        label: 'Technique',
        type: 'select',
        sourceEntity: 'technique', // 👈 carga dinámica desde /api/technique/select
        required: true
      }
    ]
  },

  exercise: {
    displayName: 'Exercises',
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      {
        name: 'difficulty',
        label: 'Difficulty',
        type: 'select',
        options: [
          { value: 'Easy', label: 'Easy' },
          { value: 'Medium', label: 'Medium' },
          { value: 'Hard', label: 'Hard' },
          { value: 'Expert', label: 'Expert' }
        ]
      },
      { name: 'bpm', label: 'BPM', type: 'number' },
      { name: 'tabNotation', label: 'Tab Notation', type: 'textarea' },
      {
        name: 'tuningId',
        label: 'Tuning',
        type: 'select',
        sourceEntity: 'tuning' // 👈 carga dinámica desde /api/tuning/select
      }
    ]
  },

  tuning: {
    displayName: 'Tunings',
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'notes', label: 'Notes', type: 'text' }
    ]
  },

  technique: {
    displayName: 'Techniques',
    fields: [
      { name: 'name', label: 'Name', type: 'text', required: true },
      { name: 'description', label: 'Description', type: 'textarea' }
    ]
  },

  user: {
    displayName: 'Users',
    useJoinEndpoint: true,
    fields: [
      { name: 'username', label: 'Username', type: 'text', required: true },
      { name: 'email', label: 'Email', type: 'email', required: true },
      { name: 'password', label: 'Password', type: 'password', required: true },
      {
        name: 'guitaristId',
        label: 'Guitarist',
        type: 'select',
        sourceEntity: 'guitarist' // 👈 carga dinámica desde /api/guitarist/select
      }
    ]
  },

  guitaristlesson: {
    displayName: 'Guitarist Lessons',
    useJoinEndpoint: true,
    fields: [
      {
        name: 'guitaristId',
        label: 'Guitarist',
        type: 'select',
        sourceEntity: 'guitarist',
        required: true
      },
      {
        name: 'lessonId',
        label: 'Lesson',
        type: 'select',
        sourceEntity: 'lesson',
        required: true
      },
      {
        name: 'status',
        label: 'Status',
        type: 'select',
        options: [
          { value: 'NotStarted', label: 'Not Started' },
          { value: 'InProgress', label: 'In Progress' },
          { value: 'Completed', label: 'Completed' }
        ]
      },
      { name: 'progressPercent', label: 'Progress (%)', type: 'number' }
    ]
  },

  lessonexercise: {
    displayName: 'Lesson Exercises',
    useJoinEndpoint: true,
    fields: [
      {
        name: 'lessonId',
        label: 'Lesson',
        type: 'select',
        sourceEntity: 'lesson',
        required: true
      },
      {
        name: 'exerciseId',
        label: 'Exercise',
        type: 'select',
        sourceEntity: 'exercise',
        required: true
      }
    ]
  }
};
