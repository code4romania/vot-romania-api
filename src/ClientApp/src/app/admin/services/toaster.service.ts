import { Injectable } from "@angular/core";
import { MatSnackBar, MatSnackBarConfig } from "@angular/material";

@Injectable({ providedIn: 'root' })
export class ToasterService {


  constructor(public snackBar: MatSnackBar) { }

  config: MatSnackBarConfig = {
    duration: 3000,
    horizontalPosition: 'right',
    verticalPosition: 'top'
  }

  show(message: string, level: 'success' | 'warning'): void {
    if (level === 'warning') {
      this.config['panelClass'] = ['notification', 'warn'];
      this.snackBar.open(message, '', this.config);
    } else {
      this.config['panelClass'] = ['notification', 'success'];
      this.snackBar.open(message, '', this.config);
    }
  }

}