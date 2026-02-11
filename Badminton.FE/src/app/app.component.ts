import {Component, OnInit} from '@angular/core';
import {map, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {AsyncPipe, JsonPipe} from "@angular/common";

@Component({
    selector: 'app-root',
    imports: [AsyncPipe, JsonPipe],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class App implements OnInit {
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