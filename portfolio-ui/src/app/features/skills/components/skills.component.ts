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
import { SkeletonComponent }   from '../../../shared/components/skeleton/skeleton.component';
import { SkillsApiService }    from '../../../core/services/skills-api.service';
import { ToastService }        from '../../../core/services/toast.service';
import {
  SkillCategory,
  SkillFilter,
  SkillGroupApiDto,
} from '../../../core/models/portfolio.models';

type LoadState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [NgClass, ObserveVisibilityDirective, SkeletonComponent],
  templateUrl: './skills.component.html',
  styleUrl: './skills.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkillsComponent implements OnInit {
  private readonly skillsApi = inject(SkillsApiService);
  private readonly toastSvc  = inject(ToastService);

  readonly headerVisible = signal(false);
  readonly gridVisible   = signal(false);

  // ── Load state ────────────────────────────────────────────────────
  readonly loadState  = signal<LoadState>('loading');
  readonly allGroups  = signal<SkillGroupApiDto[]>([]);
  readonly skeletonCards = Array(4);

  // ── Filter ────────────────────────────────────────────────────────
  readonly activeFilter = signal<SkillCategory>('all');

  readonly filters: SkillFilter[] = [
    { id: 'all',      label: 'All'      },
    { id: 'backend',  label: 'Backend'  },
    { id: 'frontend', label: 'Frontend' },
    { id: 'ai',       label: 'AI / ML'  },
    { id: 'devops',   label: 'DevOps'   },
    { id: 'database', label: 'Database' },
  ];

  // ── Computed filtered groups ──────────────────────────────────────
  readonly filteredGroups = computed<SkillGroupApiDto[]>(() => {
    const f = this.activeFilter();
    return f === 'all'
      ? this.allGroups()
      : this.allGroups().filter(g => g.category === f);
  });

  readonly totalSkillCount = computed(() =>
    this.filteredGroups().reduce((acc, g) => acc + g.skills.length, 0)
  );

  ngOnInit(): void {
    this.skillsApi.getAll().subscribe({
      next: (data) => {
        this.allGroups.set(data);
        this.loadState.set('loaded');
      },
      error: (err: Error) => {
        this.loadState.set('error');
        this.toastSvc.error('Could not load skills', err.message);
      },
    });
  }

  setFilter(id: SkillCategory): void {
    this.activeFilter.set(id);
  }

  trackGroup(_: number, g: SkillGroupApiDto): string { return g.id; }
  trackSkill(_: number, s: { name: string }): string  { return s.name; }
}
