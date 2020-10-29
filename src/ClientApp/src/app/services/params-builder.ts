import { HttpParams } from "@angular/common/http";

export class RequestHttpParams {
    private params: HttpParams;

    private constructor() {
        this.params = new HttpParams();
    }

    public static create(): RequestHttpParams {
        return new RequestHttpParams();
    }

    public append(name: string, value: string | number = undefined): RequestHttpParams {
        if (name === undefined || name.trim() === '') {
            throw new Error('Parameter should have name!');
        }

        if (value !== undefined && value.toString().trim() !== '') {
            this.params = this.params.append(name, value.toString().trim());
        }

        return this;
    }

    public please(): HttpParams {
        return this.params;
    }
}