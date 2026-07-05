import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { NgClass } from '@angular/common';
import { ObserveVisibilityDirective } from '../../../shared/directives/observe-visibility.directive';
import { SkeletonComponent }    from '../../../shared/components/skeleton/skeleton.component';
import { ProjectsApiService }   from '../../../core/services/projects-api.service';
import { ToastService }         from '../../../core/services/toast.service';
import { ProjectApiDto }        from '../../../core/models/portfolio.models';

type LoadState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [NgClass, ObserveVisibilityDirective, SkeletonComponent],
  templateUrl: './projects.component.html',
  styleUrl:    './projects.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectsComponent implements OnInit {
  private readonly projectsApi = inject(ProjectsApiService);
  private readonly toastSvc    = inject(ToastService);

  readonly headerVisible   = signal(false);
  readonly featuredVisible = signal(false);
  readonly compactVisible  = signal(false);

  // ── API state ──────────────────────────────────────────────────────────
  readonly loadState = signal<LoadState>('loading');
  readonly projects  = signal<ProjectApiDto[]>([]);

  // ── Derived ───────────────────────────────────────────────────────────
  readonly featuredProjects = computed(() =>
    this.projects().filter((_, i) => i < 2)
  );
  readonly compactProjects = computed(() =>
    this.projects().filter((_, i) => i >= 2)
  );

  readonly statusColors: Record<string, string> = {
    'live':        '#22c55e',
    'in-progress': '#f59e0b',
    'archived':    '#6b6b82',
  };

  readonly skeletonCards = Array(2);
  readonly skeletonSmall = Array(1);

  ngOnInit(): void {
    this.projectsApi.getAll().subscribe({
      next: (data) => {
        this.projects.set(data);
        this.loadState.set('loaded');
      },
      error: (err: Error) => {
        this.loadState.set('error');
        this.toastSvc.error('Could not load projects', err.message);
      },
    });
  }

  trackProject(_: number, p: ProjectApiDto): string { return p.id; }
  trackChip(_: number, chip: string): string         { return chip; }
}
