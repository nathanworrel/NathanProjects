INSERT INTO users (username, password, is_automatic) 
VALUES ('1', '1', false), 
       ('1', '1', false);

INSERT INTO strategy_type (id, name)
VALUES  (1, 'MacdStrategy'),
        (2, 'ForestStrategy');

INSERT INTO parameters (strategy_type_id, params)
VALUES  (1, '{"PeriodShort": 3, "PeriodLong": 10, "PeriodExponential": 2}');

INSERT INTO account (account_id, hash_account_id, user_id)
VALUES ('1', '2', 1);

INSERT INTO strategy (name, description, product, active, intermediate_data, intermediate_data_modified_time, amount_allocated, dry, num_stocks_holding, parameter_id, account_id)
VALUES ('Testing strategy', 'testing a macd strategy', 'TQQQ', true, '{}'::JSONB,'-infinity',200, true, 0, 1, 1);

INSERT INTO strategy_runtime (strategy_id, runtime)
VALUES (1, '09:50');

INSERT INTO strategy_secondary_product (product, use_order, strategy_id)
VALUES ('$VIX', 1, 1);

INSERT INTO trade (strategy_id, time_placed, quantity_placed, price_placed, order_number, status, side, desired_allocation)
VALUES (1, '2022-10-10 11:30:30', 4, 80.80, 1234, 1, 1, 1);
