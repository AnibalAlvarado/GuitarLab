import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  menuItems = [
    { name: 'Exercises', route: '/exercises', icon: '🎸', description: 'Manage guitar exercises' },
    { name: 'Guitarists', route: '/guitarists', icon: '👨‍🎤', description: 'Manage guitarist profiles' },
    { name: 'Lessons', route: '/lessons', icon: '📚', description: 'Manage lessons' },
    { name: 'Techniques', route: '/techniques', icon: '🎵', description: 'Manage techniques' },
    { name: 'Tunings', route: '/tunings', icon: '🎼', description: 'Manage tunings' },
    { name: 'Users', route: '/users', icon: '👥', description: 'Manage users' },

    // 🔹 NUEVAS PIVOTES
    { name: 'Guitarist Lessons', route: '/guitarist-lessons', icon: '🧩', description: 'Assign lessons to guitarists' },
    { name: 'Lesson Exercises', route: '/lesson-exercises', icon: '🎯', description: 'Link exercises to lessons' }
  ];

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  navigateTo(route: string) {
    this.router.navigate([route]);
  }

  logout() {
    this.authService.logout();
  }
}
