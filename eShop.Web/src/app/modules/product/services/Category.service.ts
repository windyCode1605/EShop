import { HttpClient } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { ICategory } from "../../../core/models/category.model";
import { environment } from "../../../my-lib/shared/enviroments/enviroment";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class CategoryService {
    categories = signal<ICategory[]>([]);
    private apiUrl = environment.api;
    constructor(private http: HttpClient) { }
    loadCategory() {
        if (this.categories().length > 0) return;

        this.http.get<any>(`${this.apiUrl}/api/Category/getCategory`)
            .subscribe(response => {
                if (response.isSuccess && response.value) {
                    this.categories.set(response.value);
                }
            });
    }
}