import {ChangeDetectionStrategy, Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {map, Observable} from "rxjs";
import {MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from "@angular/material/input";
import {TextFieldModule} from "@angular/cdk/text-field";
import {MatSelectModule} from "@angular/material/select";

@Component({
    selector: 'home-component',
    imports: [MatFormFieldModule, MatSelectModule, MatInputModule, TextFieldModule],
    providers: [
        {provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'fill'}}
    ],
    templateUrl: './home-component.html',
    styleUrl: './home-component.sass',
    changeDetection: ChangeDetectionStrategy.OnPush,
})

export class HomeComponent implements OnInit {
    public state$!: Observable<State>;

    constructor(private readonly httpClient: HttpClient) {
    }

    ngOnInit(): void {
        this.state$ = this.httpClient.get<MatchupMap>("http://localhost:5287/api/?names=a&names=b&names=c&names=d&minGames=1&courtCount=1").pipe(
            map((m): State => ({map: m}))
        )
    }
}

interface State {
    map: MatchupMap
}

type MatchupMap = Record<string, Matchup>;

interface Matchup {
    pairing1: Pairing;
    pairing2: Pairing;
}

interface Pairing {
    player1: string;
    player2: string;
}
