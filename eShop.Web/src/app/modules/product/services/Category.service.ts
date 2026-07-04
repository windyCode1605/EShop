import { HttpClient } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { ICategory } from "../../../core/models/category.model";

@Injectable({
    providedIn: 'root'
})
export class CategoryService {
    categories = signal<ICategory[]>([]);
    constructor(private http: HttpClient) { }
    loadCategory() {
        if (this.categories().length > 0) return;

        this.http.get<any>('http://localhost:5178/api/Category/getCategory')
            .subscribe(response => {
                // Backend trả về object có thuộc tính 'value' chứa danh sách
                if (response.isSuccess && response.value) {
                    this.categories.set(response.value);
                }
            });
    }
}