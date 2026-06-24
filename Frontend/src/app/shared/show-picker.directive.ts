import { Directive, ElementRef, HostListener, inject } from '@angular/core';

/**
 * Opens a native date/time input's picker (calendar/clock) when the field is
 * clicked, instead of only when the small picker icon is hit. The user can
 * still type the value directly. No-ops where showPicker is unsupported.
 */
@Directive({
  selector: 'input[appShowPicker]',
  standalone: true,
})
export class ShowPickerDirective {
  private readonly el = inject<ElementRef<HTMLInputElement>>(ElementRef);

  @HostListener('click')
  onClick(): void {
    try {
      this.el.nativeElement.showPicker?.();
    } catch {
      /* showPicker needs a user gesture / may be unsupported — ignore. */
    }
  }
}
