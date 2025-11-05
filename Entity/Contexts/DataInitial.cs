using Entity.Enums;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Entity.Contexts
{
    internal class DataInitial
    {
        public static void Data(ModelBuilder modelBuilder)
        {
            // ==========================
            // 🎸 GUITARISTS
            // ==========================
            modelBuilder.Entity<Guitarist>().HasData(
                new Guitarist { Id = 1, Name = "Aníbal Alvarado", SkillLevel = SkillLevel.Advanced, ExperienceYears = 10 },
                new Guitarist { Id = 2, Name = "Karol Natalia Osorio", SkillLevel = SkillLevel.Intermediate, ExperienceYears = 4 },
                new Guitarist { Id = 3, Name = "Yerson Stiven Cuellar", SkillLevel = SkillLevel.Beginner, ExperienceYears = 2 }
            );

            // ==========================
            // 🎵 TECHNIQUES
            // ==========================
            modelBuilder.Entity<Technique>().HasData(
                new Technique { Id = 1, Name = "Alternate Picking", Description = "Movimientos alternados de púa para mejorar velocidad y precisión." },
                new Technique { Id = 2, Name = "Legato", Description = "Técnica centrada en hammer-ons y pull-offs para lograr fluidez." },
                new Technique { Id = 3, Name = "Sweep Picking", Description = "Desplazamientos continuos de púa a través de cuerdas para ejecutar arpegios rápidos." }
            );

            // ==========================
            // 🎶 TUNINGS
            // ==========================
            modelBuilder.Entity<Tuning>().HasData(
                new Tuning { Id = 1, Name = "Standard E", Notes = "E A D G B E" },
                new Tuning { Id = 2, Name = "Drop D", Notes = "D A D G B E" },
                new Tuning { Id = 3, Name = "C Standard", Notes = "C F A# D# G C" }
            );

            // ==========================
            // 📘 LESSONS
            // ==========================
            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { Id = 1, Name = "Dominando Alternate Picking", Description = "Serie de ejercicios para dominar el picking alternado a alta velocidad.", TechniqueId = 1 },
                new Lesson { Id = 2, Name = "Legato en escalas menores", Description = "Ejercicios de legato centrados en patrones menores y modales.", TechniqueId = 2 },
                new Lesson { Id = 3, Name = "Sweep Arpegios Mayores", Description = "Práctica de arpegios mayores con técnica de sweep.", TechniqueId = 3 }
            );

            // ==========================
            // 🧩 EXERCISES
            // ==========================
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Name = "Alternate Picking en 3 cuerdas",
                    Difficulty = Difficulty.Medium,
                    BPM = 140,
                    TabNotation =
@"E|----------------|
B|----------------|
G|--0-2-4-2-0-----|
D|--0-2-4-2-0-----|
A|----------------|
E|----------------|",
                    TuningId = 1
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Legato básico en escala menor",
                    Difficulty = Difficulty.Easy,
                    BPM = 120,
                    TabNotation =
@"E|----------------|
B|----------------|
G|--5h7p5---------|
D|-------7h9p7----|
A|----------------|
E|----------------|",
                    TuningId = 1
                },
                new Exercise
                {
                    Id = 3,
                    Name = "Sweep en arpegio mayor",
                    Difficulty = Difficulty.Hard,
                    BPM = 160,
                    TabNotation =
@"E|------12p8------|
B|----10-----10----|
G|--9---------9----|
D|-----------------|
A|-----------------|
E|-----------------|",
                    TuningId = 2
                },
                new Exercise
                {
                    Id = 4,
                    Name = "Alternate Picking avanzado",
                    Difficulty = Difficulty.Expert,
                    BPM = 180,
                    TabNotation =
@"E|--0-1-2-3-4--0-1-2-3-4--|
B|------------------------|
G|------------------------|
D|------------------------|
A|------------------------|
E|------------------------|",
                    TuningId = 1
                },
                new Exercise
                {
                    Id = 5,
                    Name = "Legato rápido con saltos",
                    Difficulty = Difficulty.Hard,
                    BPM = 150,
                    TabNotation =
@"E|--7h9p7--9h11p9--|
B|------------------|
G|--6h8p6-----------|
D|------------------|
A|------------------|
E|------------------|",
                    TuningId = 1
                }
            );

            // ==========================
            // 🎓 LESSON-EXERCISE (Pivot)
            // ==========================
            modelBuilder.Entity<LessonExercise>().HasData(
                // Lesson 1 → Alternate Picking
                new LessonExercise { LessonId = 1, ExerciseId = 1 },
                new LessonExercise { LessonId = 1, ExerciseId = 4 },

                // Lesson 2 → Legato
                new LessonExercise { LessonId = 2, ExerciseId = 2 },
                new LessonExercise { LessonId = 2, ExerciseId = 5 },

                // Lesson 3 → Sweep
                new LessonExercise { LessonId = 3, ExerciseId = 3 }
            );

            // ==========================
            // 👨‍🎓 GUITARIST-LESSON (Pivot)
            // ==========================
            modelBuilder.Entity<GuitaristLesson>().HasData(
                new GuitaristLesson { GuitaristId = 1, LessonId = 1, Status = LessonStatus.Completed, ProgressPercent = 100 },
                new GuitaristLesson { GuitaristId = 1, LessonId = 2, Status = LessonStatus.InProgress, ProgressPercent = 60 },
                new GuitaristLesson { GuitaristId = 2, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 40 },
                new GuitaristLesson { GuitaristId = 2, LessonId = 3, Status = LessonStatus.NotStarted, ProgressPercent = 0 },
                new GuitaristLesson { GuitaristId = 3, LessonId = 2, Status = LessonStatus.NotStarted, ProgressPercent = 0 }
            );
        }
    }
}
