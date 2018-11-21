import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { SearchService } from 'app/shared/services/search.service';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Observable } from 'rxjs/Observable';
import { DpgOnBoardModel } from '../models';


@Injectable()
export class DpgService {
    private dpgTypeUrl = 'api/dpgType';
    private dpgUrl = 'api/dpg';
    private searchUrl = 'api/dpg/search';
    private dpgOnBoardUrl = 'api/dpgOnBoard';
    private dpgOnBoardByPortCallUrl = 'api/dpgOnBoard/portcall';
    private measurementTypeFilterUrl = 'api/measurementType/filter';
    private searchService: SearchService;

    constructor(private http: Http, private httpClient: HttpClient) {
        this.searchService = new SearchService(this.httpClient);
    }

    private dpgOnBoardSource = new BehaviorSubject<DpgOnBoardModel[]>(null);
    dpgOnBoardList$ = this.dpgOnBoardSource.asObservable();

    private dataIsPristine = new BehaviorSubject<boolean>(true);
    dataIsPristine$ = this.dataIsPristine.asObservable();

    private dpgIsChecked = new BehaviorSubject<boolean>(false);
    dpgIsChecked$ = this.dpgIsChecked.asObservable();

    setDpgOnBoardList(data) {
        this.dpgOnBoardSource.next(data);
    }
    setDataIsPristine(isPristine: boolean) {
        this.dataIsPristine.next(isPristine);
    }

    setDpgCheckedInProgressBar(checked: boolean) {
        this.dpgIsChecked.next(checked);
    }
    // Http
    getDpgTypes() {
        const uri = this.dpgTypeUrl + '/all';
        return this.http.get(uri).map(res => res.json());
    }

    getDpgById(DpgId: number) {
        const uri = [this.dpgUrl, DpgId].join('/');
        return this.http.get(uri).map(res => res.json());
    }

    getDpgOnBoardListByPortCallId(portCallId: number) {
        const uri = [this.dpgOnBoardByPortCallUrl, portCallId].join('/');
        return this.http.get(uri).map(res => res.json());
    }

    getMeasurementTypeList(filter: string) {
        const uri = this.measurementTypeFilterUrl;
        return this.http.get(uri).map(res => res.json());
      }

    search(dpgType: number, term: string, amount = 10) {
        if (term.length < 2) {
            return Observable.of([]);
        }
        return this.searchService.search(this.searchUrl, term, amount, dpgType);
    }

    saveDpgOnBoard(dpgOnBoardList: any[], portCallId: number) {
        const uri = [this.dpgOnBoardUrl, portCallId, '/list'].join('/');
        return this.http.put(uri, dpgOnBoardList);
    }

    formatDpgOnBoard(dpgOnBoardList: DpgOnBoardModel[], portCallId: number): DpgOnBoardModel[] {
        let formattedList: DpgOnBoardModel[] = [];
        if (dpgOnBoardList && dpgOnBoardList.length > 0) {
            formattedList = dpgOnBoardList.map(
                item => {
                    const formattedDpg = new DpgOnBoardModel();
                    formattedDpg.portCallId = portCallId;
                    formattedDpg.dpgId = item.dpgId;
                    formattedDpg.dpgOnBoardId = item.dpgOnBoardId;
                    formattedDpg.grossWeight = item.grossWeight;
                    formattedDpg.locationOnBoard = item.locationOnBoard;
                    formattedDpg.netWeight = item.netWeight;
                    formattedDpg.placedInContainer = item.placedInContainer;
                    formattedDpg.transportUnitIdentification = item.transportUnitIdentification;
                    formattedDpg.measurementTypeId = item.measurementTypeId;
                    return formattedDpg;
                }
            );
        }
        return formattedList;
    }
}