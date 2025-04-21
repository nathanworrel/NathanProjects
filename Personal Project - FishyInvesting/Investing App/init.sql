-- TODO: work on encryption etc in DB
CREATE TABLE IF NOT EXISTS users
(
    id           SERIAL PRIMARY KEY,
    username     varchar(50) not null,
    password     varchar(50) not null,
    is_automatic bool
);

CREATE TABLE IF NOT EXISTS auth_token
(
    id                       SERIAL PRIMARY KEY,
    user_id                  int,
    access_token             varchar(1000) not null,
    access_token_expiration  Timestamp     not null,
    refresh_token            varchar(1000) not null,
    refresh_token_expiration Timestamp     not null,
    authorization_code       varchar(1000) not null,
    app_key                  varchar(100),
    app_secret               varchar(100),
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users (id)
);

CREATE TABLE IF NOT EXISTS pricing_data
(
    product varchar(10),
    time    TimeStamp,
    price   decimal(10, 2),
    CONSTRAINT unique_product_time_price UNIQUE (product, time)
)
    PARTITION BY LIST (product);

CREATE TABLE pricing_data_tqqq
    PARTITION OF pricing_data
        FOR VALUES IN ('TQQQ');

CREATE TABLE pricing_data_qqq
    PARTITION OF pricing_data
        FOR VALUES IN ('QQQ');

CREATE TABLE pricing_data_vix
    PARTITION OF pricing_data
        FOR VALUES IN ('$VIX');

CREATE TABLE pricing_data_default
    PARTITION OF pricing_data
        DEFAULT;

CREATE TABLE IF NOT EXISTS strategy_type
(
    id   serial PRIMARY KEY,
    name varchar(100) NOT NULL,
    CONSTRAINT unique_strategy_type UNIQUE (name)
);

CREATE TABLE IF NOT EXISTS parameters
(
    id               serial PRIMARY KEY,
    strategy_type_id int  NOT NULL,
    params           json NOT NULL,
    CONSTRAINT fk_strategy_type FOREIGN KEY (strategy_type_id) REFERENCES strategy_type (id)
);

CREATE TABLE IF NOT EXISTS account
(
    id              serial PRIMARY KEY,
    account_id      varchar(100) not null,
    hash_account_id varchar(200) not null,
    user_id         int          NOT NULL,
    CONSTRAINT fk_user_id FOREIGN KEY (user_id) REFERENCES users (id)
);

CREATE TABLE IF NOT EXISTS strategy
(
    id                              serial PRIMARY KEY,
    name                            varchar(50),
    description                     varchar(100),
    product                         varchar(5) NOT NULL,
    active                          bool       NOT NULL DEFAULT FALSE,
    intermediate_data               json,
    intermediate_data_modified_time timestamp,
    amount_allocated                decimal    NOT NULL,
    dry                             bool       NOT NULL DEFAULT TRUE,
    num_stocks_holding              int        NOT NULL DEFAULT 0,
    parameter_id                    int        NOT NULL,
    account_id                      int        NOT NULL,
    CONSTRAINT fk_parameters FOREIGN KEY (parameter_id) REFERENCES parameters (id),
    CONSTRAINT fk_accounts FOREIGN KEY (account_id) REFERENCES account (id)
);

CREATE TABLE IF NOT EXISTS strategy_runtime
(
    id          serial PRIMARY KEY,
    strategy_id int  NOT NULL,
    runtime     time NOT NULL,
    CONSTRAINT fk_strategy FOREIGN KEY (strategy_id) REFERENCES strategy (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS strategy_secondary_product
(
    id          serial PRIMARY KEY,
    product     varchar(5) NOT NULL,
    use_order   int        NOT NULL,
    strategy_id int        NOT NULL,
    CONSTRAINT fk_strategy FOREIGN KEY (strategy_id) REFERENCES strategy (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS trade
(
    id              serial PRIMARY KEY,
    strategy_id     int            NOT NULL,
    time_placed     timestamp      NOT NULL DEFAULT now(),
    quantity_placed int            NOT NULL,
    price_placed    decimal(10, 2) NOT NULL,
    order_number    bigint         NOT NULL,
    status          int            NOT NULL,
    side            int            NOT NULL,
    desired_allocation decimal(10,2)     NOT NULL,
    time_modified   timestamp,
    quantity_filled int            NOT NULL DEFAULT 0,
    price_filled    decimal(10, 2) NOT NULL DEFAULT 0,
    CONSTRAINT fk_strategy FOREIGN KEY (strategy_id) REFERENCES strategy (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS market_time
(
    id      serial PRIMARY KEY,
    date    timestamp    NOT NULL,
    is_open boolean NOT NULL,
    open    time,
    close   time
);