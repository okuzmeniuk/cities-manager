import { Directive, Input } from '@angular/core';
import { FormControl, NgControl } from '@angular/forms';

@Directive({
  selector: '[disableControl]',
  standalone: true,
})
export class DisableControlDirective {
  constructor(private ngControl: NgControl) {}

  @Input()
  set disableControl(condition: boolean) {
    const control = this.ngControl.control as FormControl;

    if (control) {
      condition ? control.disable() : control.enable();
    }
  }
}
