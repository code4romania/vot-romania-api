import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'joinTranslations'
})
export class JoinTranslationsPipe implements PipeTransform {

  transform(input: any, sep = ','): string {
    const values = Object.values(input);
    return values.join(sep);
  }

}
