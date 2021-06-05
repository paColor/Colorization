import PhonConfig from "./PhonConfig";

export default class Config {
    public readonly pc: PhonConfig;

    constructor() {
        this.pc = new PhonConfig();
    }
}