import { Injectable } from '@angular/core';

// cdk
import { Overlay } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { MatSpinner } from '@angular/material';

// rxjs
import { Subject } from 'rxjs';
import { scan, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SpinnerService {

  private spinnerTopRef = this.cdkSpinnerCreate();

  private spin$: Subject<boolean> = new Subject();

  constructor(private overlay: Overlay) {
    this.spin$
      .asObservable()
      .pipe(
        map(val => val ? 1 : -1),
        scan((acc, one) => (acc + one) >= 0 ? acc + one : 0, 0)
      )
      .subscribe(
        (res) => {
          if (res === 1) {
            this.showSpinner();
          } else {
            if (res === 0) {
              this.spinnerTopRef.hasAttached() ? this.stopSpinner() : null;
            }
          }
        }
      );
  }

  public display(isDisplayed: boolean): void {
    this.spin$.next(isDisplayed);
  }

  private cdkSpinnerCreate() {
    return this.overlay.create({
      hasBackdrop: true,
      backdropClass: 'dark-backdrop',
      positionStrategy: this.overlay.position()
        .global()
        .centerHorizontally()
        .centerVertically()
    });
  }

  private showSpinner() {
    if (!this.spinnerTopRef.hasAttached()) {
      this.spinnerTopRef.attach(new ComponentPortal(MatSpinner));
    }
  }

  private stopSpinner() {
    this.spinnerTopRef.detach();
  }
}
