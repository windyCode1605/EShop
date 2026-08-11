import { Pipe, PipeTransform } from '@angular/core';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';

@Pipe({
  name: 'imageUrl',
  standalone: true
})
export class ImageUrlPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) {
      // You can return a default placeholder image here if desired
      return '';
    }

    // If the value is already a full URL (http or https), return it directly
    if (value.startsWith('http://') || value.startsWith('https://')) {
      return value;
    }

    // Ensure the API base URL doesn't have a trailing slash and the value has a leading slash
    const baseUrl = environment.api.replace(/\/$/, '');
    const imagePath = value.startsWith('/') ? value : `/${value}`;

    return `${baseUrl}${imagePath}`;
  }
}
