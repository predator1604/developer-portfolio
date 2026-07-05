import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  NgZone,
  OnDestroy,
  OnInit,
  PLATFORM_ID,
  inject,
  signal,
  viewChild,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ScrollSpyService } from '../../../core/services/scroll-spy.service';

interface Particle {
  x: number;
  y: number;
  vx: number;
  vy: number;
  radius: number;
  opacity: number;
}

interface NavLink {
  label: string;
  fragment: string;
}

@Component({
  selector: 'app-hero',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './hero.component.html',
  styleUrl: './hero.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly ngZone = inject(NgZone);
  private readonly scrollSpy = inject(ScrollSpyService);

  // Exposed to template for active nav link highlighting
  readonly activeSection = this.scrollSpy.activeSection;

  readonly canvasRef = viewChild.required<ElementRef<HTMLCanvasElement>>('particleCanvas');

  // ── Signals ──────────────────────────────────────────────────────────
  readonly typedText = signal('');
  readonly showCursor = signal(true);
  readonly contentVisible = signal(false);
  readonly navOpen = signal(false);

  // ── Config ───────────────────────────────────────────────────────────
  readonly navLinks: NavLink[] = [
    { label: 'About',    fragment: 'about' },
    { label: 'Projects', fragment: 'projects' },
    { label: 'Skills',   fragment: 'skills' },
    { label: 'Contact',  fragment: 'contact' },
  ];

  readonly techChips = [
    'Agentic AI',
    'RAG · MCP',
    '.NET Core',
    'Angular 19',
    'MongoDB',
    'Docker · K8s',
  ];

  private readonly roles = [
    'Full-Stack AI Engineer',
    '.NET & Angular Architect',
    'Agentic AI Builder',
    'DevOps Automation Expert',
  ];

  // ── Canvas & Animation state ──────────────────────────────────────────
  private ctx!: CanvasRenderingContext2D;
  private particles: Particle[] = [];
  private animationId = 0;
  private resizeObserver!: ResizeObserver;
  private typingTimeouts: ReturnType<typeof setTimeout>[] = [];
  private cursorInterval!: ReturnType<typeof setInterval>;
  private roleIndex = 0;
  private charIndex = 0;
  private isDeleting = false;

  // ── Particle config ───────────────────────────────────────────────────
  private readonly PARTICLE_COUNT = 80;
  private readonly CONNECTION_DISTANCE = 120;
  private readonly PARTICLE_SPEED = 0.35;
  private readonly ACCENT = { r: 99, g: 102, b: 241 };

  ngOnInit(): void {
    const t = setTimeout(() => this.contentVisible.set(true), 300);
    this.typingTimeouts.push(t);
  }

  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    this.initCanvas();
    this.startCursorBlink();
    this.startTypingLoop();
  }

  ngOnDestroy(): void {
    cancelAnimationFrame(this.animationId);
    clearInterval(this.cursorInterval);
    this.typingTimeouts.forEach(clearTimeout);
    this.resizeObserver?.disconnect();
  }

  toggleNav(): void {
    this.navOpen.update(v => !v);
  }

  // ── Canvas ─────────────────────────────────────────────────────────────
  private initCanvas(): void {
    const canvas = this.canvasRef().nativeElement;
    const context = canvas.getContext('2d');
    if (!context) return;
    this.ctx = context;

    this.sizeCanvas(canvas);
    this.spawnParticles(canvas);

    this.resizeObserver = new ResizeObserver(() => {
      this.sizeCanvas(canvas);
      this.particles = [];
      this.spawnParticles(canvas);
    });
    this.resizeObserver.observe(canvas.parentElement!);

    this.ngZone.runOutsideAngular(() => this.renderLoop());
  }

  private sizeCanvas(canvas: HTMLCanvasElement): void {
    const parent = canvas.parentElement!;
    canvas.width  = parent.offsetWidth;
    canvas.height = parent.offsetHeight;
  }

  private spawnParticles(canvas: HTMLCanvasElement): void {
    this.particles = Array.from({ length: this.PARTICLE_COUNT }, () => ({
      x:       Math.random() * canvas.width,
      y:       Math.random() * canvas.height,
      vx:      (Math.random() - 0.5) * this.PARTICLE_SPEED,
      vy:      (Math.random() - 0.5) * this.PARTICLE_SPEED,
      radius:  Math.random() * 1.8 + 0.4,
      opacity: Math.random() * 0.6 + 0.2,
    }));
  }

  private renderLoop(): void {
    const canvas = this.canvasRef().nativeElement;
    const { width: W, height: H } = canvas;
    const { r, g, b } = this.ACCENT;

    this.ctx.clearRect(0, 0, W, H);

    for (const p of this.particles) {
      p.x += p.vx;
      p.y += p.vy;
      if (p.x < 0 || p.x > W) p.vx *= -1;
      if (p.y < 0 || p.y > H) p.vy *= -1;

      this.ctx.beginPath();
      this.ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
      this.ctx.fillStyle = `rgba(${r},${g},${b},${p.opacity})`;
      this.ctx.fill();
    }

    for (let i = 0; i < this.particles.length; i++) {
      for (let j = i + 1; j < this.particles.length; j++) {
        const a = this.particles[i];
        const bP = this.particles[j];
        const dx = a.x - bP.x;
        const dy = a.y - bP.y;
        const dist = Math.sqrt(dx * dx + dy * dy);

        if (dist < this.CONNECTION_DISTANCE) {
          const alpha = (1 - dist / this.CONNECTION_DISTANCE) * 0.18;
          this.ctx.beginPath();
          this.ctx.moveTo(a.x, a.y);
          this.ctx.lineTo(bP.x, bP.y);
          this.ctx.strokeStyle = `rgba(${r},${g},${b},${alpha})`;
          this.ctx.lineWidth = 0.6;
          this.ctx.stroke();
        }
      }
    }

    this.animationId = requestAnimationFrame(() => this.renderLoop());
  }

  // ── Typing effect ──────────────────────────────────────────────────────
  private startTypingLoop(): void {
    const tick = () => {
      const current = this.roles[this.roleIndex];

      if (this.isDeleting) {
        this.charIndex--;
        this.typedText.set(current.slice(0, this.charIndex));

        if (this.charIndex === 0) {
          this.isDeleting = false;
          this.roleIndex  = (this.roleIndex + 1) % this.roles.length;
          const t = setTimeout(tick, 500);
          this.typingTimeouts.push(t);
          return;
        }
        const t = setTimeout(tick, 40);
        this.typingTimeouts.push(t);
      } else {
        this.charIndex++;
        this.typedText.set(current.slice(0, this.charIndex));

        if (this.charIndex === current.length) {
          this.isDeleting = true;
          const t = setTimeout(tick, 2200);
          this.typingTimeouts.push(t);
          return;
        }
        const t = setTimeout(tick, 80);
        this.typingTimeouts.push(t);
      }
    };

    const init = setTimeout(tick, 900);
    this.typingTimeouts.push(init);
  }

  private startCursorBlink(): void {
    this.cursorInterval = setInterval(
      () => this.showCursor.update(v => !v),
      530
    );
  }
}
