import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProjectApiDto } from '../models/portfolio.models';

@Injectable({ providedIn: 'root' })
export class ProjectsApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/projects`;

  getAll(): Observable<ProjectApiDto[]> {
    return this.http.get<ProjectApiDto[]>(this.base).pipe(
      catchError(() => throwError(() => new Error('Failed to load projects.')))
    );
  }

  getBySlug(slug: string): Observable<ProjectApiDto> {
    return this.http.get<ProjectApiDto>(`${this.base}/${slug}`).pipe(
      catchError(() => throwError(() => new Error(`Project '${slug}' not found.`)))
    );
  }
}
