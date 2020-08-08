import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class ToasterService {

  show(message: string, level: 'success' | 'warning' | 'error'): void {
    alert(`${level}: ${message}`);
  }

}