import { Component, computed, input, output, signal } from '@angular/core';

/**
 * A text field with a type-ahead suggestion list that drops down *below* the
 * input (not a native <datalist>). Suggestions are filtered case-insensitively
 * as the user types. Emits `valueChange` on every keystroke/selection and
 * `selected` when a suggestion is chosen or Enter is pressed.
 */
@Component({
  selector: 'app-autocomplete',
  standalone: true,
  template: `
    <div class="ac-wrap">
      <input #inp class="input" type="text" autocomplete="off"
             [id]="inputId()"
             [value]="value()"
             [placeholder]="placeholder()"
             (input)="onInput(inp.value)"
             (focus)="open.set(true)"
             (blur)="onBlur()"
             (keydown)="onKey($event)" />
      @if (open() && filtered().length) {
        <ul class="ac-list">
          @for (s of filtered(); track s; let i = $index) {
            <li class="ac-item" [class.active]="i === highlight()"
                (mousedown)="$event.preventDefault()"
                (click)="choose(s)"
                (mouseenter)="highlight.set(i)">{{ s }}</li>
          }
        </ul>
      }
    </div>
  `,
  styles: [`
    :host { display: block; }
    .ac-wrap { position: relative; }
    .ac-list {
      position: absolute; top: calc(100% + 4px); left: 0; right: 0; z-index: 60;
      margin: 0; padding: 4px; list-style: none;
      background: #fff; border: 1px solid var(--line); border-radius: 10px;
      box-shadow: 0 12px 30px rgba(15, 23, 42, 0.16);
      max-height: 260px; overflow-y: auto;
    }
    .ac-item {
      padding: 8px 12px; border-radius: 7px; cursor: pointer; font-size: 14px;
      white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
    }
    .ac-item.active, .ac-item:hover { background: var(--row-hover); }
  `],
})
export class Autocomplete {
  readonly value = input<string>('');
  readonly placeholder = input<string>('');
  readonly suggestions = input<readonly string[]>([]);
  readonly inputId = input<string>('');
  readonly limit = input<number>(10);

  readonly valueChange = output<string>();
  readonly selected = output<string>();

  readonly open = signal(false);
  readonly highlight = signal(-1);

  readonly filtered = computed(() => {
    const q = this.value().trim().toLowerCase();
    const all = this.suggestions();
    const matches = q ? all.filter(s => s.toLowerCase().includes(q)) : all.slice();
    return matches.slice(0, this.limit());
  });

  onInput(v: string): void {
    this.valueChange.emit(v);
    this.open.set(true);
    this.highlight.set(-1);
  }

  onKey(e: KeyboardEvent): void {
    const items = this.filtered();
    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        this.open.set(true);
        this.highlight.update(h => Math.min(h + 1, items.length - 1));
        break;
      case 'ArrowUp':
        e.preventDefault();
        this.highlight.update(h => Math.max(h - 1, -1));
        break;
      case 'Enter': {
        const h = this.highlight();
        if (this.open() && h >= 0 && h < items.length) {
          e.preventDefault();           // pick the highlighted suggestion, don't submit
          this.choose(items[h]);
        } else {
          this.open.set(false);         // let a host <form> submit on Enter
          this.selected.emit(this.value());
        }
        break;
      }
      case 'Escape':
        this.open.set(false);
        this.highlight.set(-1);
        break;
    }
  }

  choose(s: string): void {
    this.valueChange.emit(s);
    this.selected.emit(s);
    this.open.set(false);
    this.highlight.set(-1);
  }

  onBlur(): void {
    // Delay so a click on a suggestion registers before the list closes.
    setTimeout(() => this.open.set(false), 120);
  }
}
