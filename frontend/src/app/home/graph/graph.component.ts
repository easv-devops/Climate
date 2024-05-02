import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {
  ApexAnnotations,
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexMarkers,
  ApexStroke,
  ApexTitleSubtitle,
  ApexTooltip,
  ApexXAxis,
  ApexYAxis,
  ChartComponent
} from "ng-apexcharts";
//import {data} from "./series-data";
import {SensorDto} from "../../../models/Entities";


export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers;
  title: ApexTitleSubtitle;
  fill: ApexFill;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  annotations: ApexAnnotations;
  colors: any;
  toolbar: any;
};

@Component({
  selector: 'app-graph',
  templateUrl: './graph.component.html',
  styleUrls: ['./graph.component.scss'],
})
export class GraphComponent implements OnInit{
  @Input() readings!: SensorDto[];
  @ViewChild("chart", { static: false }) chart!: ChartComponent;
  chartOptions: any = {};
  public activeOptionButton = "all";
  public updateOptionsData = {
    "1m": {
      xaxis: {
        min: new Date("28 Jan 2013").getTime(),
        max: new Date("27 Feb 2013").getTime()
      }
    },
    "6m": {
      xaxis: {
        min: new Date("27 Sep 2012").getTime(),
        max: new Date("27 Feb 2013").getTime()
      }
    },
    "1y": {
      xaxis: {
        min: new Date("27 Feb 2012").getTime(),
        max: new Date("27 Feb 2013").getTime()
      }
    },
    "1yd": {
      xaxis: {
        min: new Date("01 Jan 2013").getTime(),
        max: new Date("27 Feb 2013").getTime()
      }
    },
    all: {
      xaxis: {
        min: undefined,
        max: undefined
      }
    }
  };

  constructor() {
    this.initChart();
  }

  initChart(): void {
    this.chartOptions = {
      series: [
        {
          data: []
        }
      ],
      chart: {
        type: "area",
        height: 350
      },
      dataLabels: {
        enabled: false
      },
      markers: {
        size: 0
      },
      xaxis: {
        type: "datetime",
        min: new Date("01 Mar 2012").getTime(),
        tickAmount: 6
      },
      tooltip: {
        x: {
          format: "dd MMM yyyy"
        }
      },
      fill: {
        type: "gradient",
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.7,
          opacityTo: 0.9,
          stops: [0, 100]
        }
      }
    };
  }

  public updateOptions(option: any): void {
    this.activeOptionButton = option;
    // @ts-ignore
    this.chart.updateOptions(this.updateOptionsData[option], false, true, true);
  }

  updateGraph(option: string) {
    // Logic to update the graph based on the selected option
    // For example:
    if (option === 'temperature') {
      // Update graph for temperature
    } else if (option === 'humidity') {
      // Update graph for humidity
    } else if (option === 'pm') {
      // Update graph for PM
    }
  }

  // TODO: logic should not be placed here, testing if this is the issue
  private convertTimestamp() {
    // Make sure readings is initialized and is an array
    if (Array.isArray(this.readings)) {
      for (const r of this.readings) {
        r.TimeStamp = new Date(r.TimeStamp).getTime();
      }
    }
  }

  private updateChartData() {
    // Assuming SensorDto has properties for x and y axes data
    if (Array.isArray(this.readings)) {
      // Map readings data to chart data format for each series
      this.chartOptions.series = this.readings.map((reading, index) => ({
        data: [
          {
            x: reading.TimeStamp, // Assuming TimeStamp is your x-axis data
            y: reading.Value // Replace Value with actual property name for y-axis
          }
        ]
      }));
    }

    this.updateOptions(null);
  }


  ngOnInit(): void {
    this.convertTimestamp();
    this.updateChartData();
  }
}
