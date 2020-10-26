import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatDistance'
})
export class FormatDistancePipe implements PipeTransform {

  transform(value: number): string {
    if (value > 1000) {
      return `${Math.round(value / 1000)} km`;
    }

    return `${Math.round(value)} m`;
  }

}
