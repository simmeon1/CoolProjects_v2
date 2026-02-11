import {Component} from '@angular/core';
import {HomeComponent} from "./home-component/home-component";

@Component({
    selector: 'app-root',
    imports: [
        HomeComponent
    ],
    template: `
        <home-component></home-component>`,
    styleUrls: ['./app.css'],
})
export class App {
    title = 'default';
}
