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
            //  USERS
            // ==========================
            modelBuilder.Entity<User>().HasData(
                // password: Anibal123!
                new User { Id = 1, Username = "anibal25", Password = "$2a$12$tWJFKE5AxcI22akEUprqMuJoQ0QXLUbAg4FRklLYTQQc1dGCv1AlW", Email="anibalalvaradoandrade@gmail.com", GuitaristId = 1 },
                // password: Carva123!
                new User { Id = 2, Username = "carvaInsane", Password = "$2a$12$AyTpx8lEKlO9F9ltw5b8j.hGYJ44gK5pFL.ATJNYshNP6PswzOpPa", Email = "carva@gmail.com", GuitaristId = 2 },
                // password: Viego123!
                new User { Id = 3, Username = "viego", Password = "$2a$12$qq6J.RdSq1OTx8hLUGI5huNP9eL.zwD/Ev0QA2ZJ6W3z1YBW0Yo4W", Email = "viego@gmail.com", GuitaristId = 3 }
            );
            // ==========================

            // ==========================
            // 🎸 GUITARISTS
            // ==========================
            modelBuilder.Entity<Guitarist>().HasData(
                new Guitarist { Id = 1, Name = "Aníbal Alvarado Andrade", SkillLevel = SkillLevel.Advanced, ExperienceYears = 5 },
                new Guitarist { Id = 2, Name = "Brayan Stiven Carvajal", SkillLevel = SkillLevel.Intermediate, ExperienceYears = 1 },
                new Guitarist { Id = 3, Name = "Diego Fernando Cuellar", SkillLevel = SkillLevel.Beginner, ExperienceYears = 1 }
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
                new LessonExercise { Id = 1, LessonId = 1, ExerciseId = 1 },
                new LessonExercise { Id = 2, LessonId = 1, ExerciseId = 4 },

                // Lesson 2 → Legato
                new LessonExercise { Id = 3, LessonId = 2, ExerciseId = 2 },
                new LessonExercise { Id = 4, LessonId = 2, ExerciseId = 5 },

                // Lesson 3 → Sweep
                new LessonExercise { Id = 5, LessonId = 3, ExerciseId = 3 }
            );

            // ==========================
            // 👨‍🎓 GUITARIST-LESSON (Pivot)
            // ==========================
            modelBuilder.Entity<GuitaristLesson>().HasData(
                new GuitaristLesson { Id = 1, GuitaristId = 1, LessonId = 1, Status = LessonStatus.Completed, ProgressPercent = 100 },
                new GuitaristLesson { Id = 2, GuitaristId = 1, LessonId = 2, Status = LessonStatus.InProgress, ProgressPercent = 60 },
                new GuitaristLesson { Id = 3, GuitaristId = 2, LessonId = 1, Status = LessonStatus.InProgress, ProgressPercent = 40 },
                new GuitaristLesson { Id = 4, GuitaristId = 2, LessonId = 3, Status = LessonStatus.NotStarted, ProgressPercent = 0 },
                new GuitaristLesson { Id = 5, GuitaristId = 3, LessonId = 2, Status = LessonStatus.NotStarted, ProgressPercent = 0 }
            );
        }
    }
}
