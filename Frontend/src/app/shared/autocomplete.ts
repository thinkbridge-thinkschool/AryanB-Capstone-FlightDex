import { Component, computed, input, output, signal } from '@angular/core';

/** A selectable suggestion: `label` is shown and filtered on; `value` is emitted when picked. */
export interface AutocompleteOption {
  label: string;
  value: string;
}

/**
 * A text field with a type-ahead suggestion list that drops down *below* the
 * input (not a native <datalist>). Each option's `label` is shown in the list and
 * matched case-insensitively against what the user types; picking one emits its
 * `value` via `optionPicked`. `valueChange` fires on every keystroke (raw text) and
 * `submit` fires when Enter is pressed without a suggestion highlighted.
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
          @for (o of filtered(); track o.label; let i = $index) {
            <li class="ac-item" [class.active]="i === highlight()"
                (mousedown)="$event.preventDefault()"
                (click)="choose(o)"
                (mouseenter)="highlight.set(i)">{{ o.label }}</li>
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
  readonly options = input<readonly AutocompleteOption[]>([]);
  readonly inputId = input<string>('');
  readonly limit = input<number>(10);
  /** Minimum characters typed before suggestions appear. 0 shows all options on focus. */
  readonly minChars = input<number>(0);

  /** Raw input text on every keystroke. */
  readonly valueChange = output<string>();
  /** A suggestion was chosen (click or Enter on a highlighted item). */
  readonly optionPicked = output<AutocompleteOption>();
  /** Enter pressed with no suggestion highlighted — carries the raw text. */
  readonly submit = output<string>();

  readonly open = signal(false);
  readonly highlight = signal(-1);

  readonly filtered = computed(() => {
    const q = this.value().trim().toLowerCase();
    // Below the minimum length, show nothing (e.g. wait for the first typed letter).
    if (q.length < this.minChars()) return [];
    const all = this.options();
    const matches = q ? all.filter(o => o.label.toLowerCase().includes(q)) : all.slice();
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
          this.submit.emit(this.value());
        }
        break;
      }
      case 'Escape':
        this.open.set(false);
        this.highlight.set(-1);
        break;
    }
  }

  choose(o: AutocompleteOption): void {
    this.optionPicked.emit(o);
    this.open.set(false);
    this.highlight.set(-1);
  }

  onBlur(): void {
    // Delay so a click on a suggestion registers before the list closes.
    setTimeout(() => this.open.set(false), 120);
  }
}
