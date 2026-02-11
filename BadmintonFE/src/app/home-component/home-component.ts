import {ChangeDetectionStrategy, Component} from '@angular/core';

@Component({
    selector: 'home-component',
    imports: [],
    templateUrl: './home-component.html',
    styleUrl: './home-component.sass',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomeComponent {

}
