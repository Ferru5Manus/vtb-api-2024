using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using EntertaimentService.Kafka;
using EntertaimentService.Kafka.Utils;
using EntertaimentService.KafkaException;
using EntertaimentService.KafkaException.ConsumerException;
using EntertaimentService.Models.Review.Requests;
using EntertaimentService.Models.Review.Responses;
using EntertaimentService.Services.ReviewService;
using EntertaimentService.TourReview.Requests;

namespace EntertaimentService.KafkaServices
{
    public class KafkaReviewService : KafkaService
    {
        private readonly string _reviewRequestTopic = Environment.GetEnvironmentVariable("REVIEW_REQUEST_TOPIC") ?? "reviewRequestTopic";
        private readonly string _reviewResponseTopic = Environment.GetEnvironmentVariable("REVIEW_RESPONSE_TOPIC") ?? "reviewResponseTopic"; 
        private readonly IReviewService _reviewService;

        public KafkaReviewService(ILogger<KafkaService> logger, IProducer<string, string> producer, KafkaTopicManager kafkaTopicManager, IReviewService reviewService) : base(logger, producer, kafkaTopicManager)
        {
            _reviewService = reviewService;
            base.ConfigureConsumer(_reviewRequestTopic);
        }
        public override async Task Consume()
        {
            try
            {
               
                while (true)
                {
                    if(_consumer == null)
                    {
                        _logger.LogError("Consumer is null");
                        throw new ConsumerException("Consumer is null");
                    }
                    ConsumeResult<string, string> consumeResult = _consumer.Consume();
                    if (consumeResult != null)
                    {
                        var headerBytes = consumeResult.Message.Headers
                        .FirstOrDefault(x => x.Key.Equals("method")) ?? throw new NullReferenceException("headerBytes is null");
                    
                  
                        var methodString = Encoding.UTF8.GetString(headerBytes.GetValueBytes());
                        switch (methodString)
                        {
                            case "addReview":
                                try
                                {
                                    var request = JsonConvert.DeserializeObject<AddReviewRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(request))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(new AddReviewResponse() 
                                            {
                                                ReviewId = await _reviewService.AddReview(request)
                                            }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("addReview")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Request validation error");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("addReview")), 
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")), 
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }

                                break;
                            case "getReview":
                                try
                                {
                                    var result = JsonConvert.DeserializeObject<GetReviewRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(result))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(
                                                new GetReviewResponse(){ 
                                                    Review = await _reviewService.GetReview(result)
                                                    }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("getReview")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Invalid request");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("getReview")), 
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")), 
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }
                                break;
                            case "getReviews":
                                try
                                {
                                    var result = JsonConvert.DeserializeObject<GetReviewsRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(result))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(
                                                new GetReviewsResponse(){ 
                                                    Reviews = _reviewService.GetReviews(result).ToList()
                                                }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("getReviews")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Invalid request");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("getReviews")), 
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")), 
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }
                                break;
                            case "removeReview":
                                try
                                {
                                    var result = JsonConvert.DeserializeObject<RemoveReviewRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(result))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(
                                                new RemoveReviewResponse(){ 
                                                    IsSuccess = _reviewService.RemoveReview(result)
                                                }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("removeReview")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Invalid request");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("removeReview")), 
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")), 
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }
                                break;
                            case "updateReview":
                                try
                                {
                                    var result = JsonConvert.DeserializeObject<UpdateReviewRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(result))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(
                                                new UpdateReviewResponse(){
                                                     IsSuccess =_reviewService.UpdateReview(result)
                                                    }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("updateReview")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Invalid request");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("updateReview")),
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")),
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }
                                break;
                            case "rateReview":
                                try
                                {
                                    var result = JsonConvert.DeserializeObject<RateReviewRequest>(consumeResult.Message.Value) ?? throw new NullReferenceException("result is null");
                                    if(base.IsValid(result))
                                    {
                                        if(await base.Produce(_reviewResponseTopic,new Message<string, string>()
                                        {
                                            Key = consumeResult.Message.Key,
                                            Value = JsonConvert.SerializeObject(
                                                new RateReviewResponse(){
                                                     IsSuccess =await _reviewService.RateReview(result)
                                                    }),
                                            Headers = [
                                                new Header("method",Encoding.UTF8.GetBytes("rateReview")),
                                                new Header("sender",Encoding.UTF8.GetBytes("entertaimentService"))
                                            ]
                                        }))
                                        {

                                            _logger.LogInformation("Successfully sent message {Key}",consumeResult.Message.Key);
                                            _consumer.Commit(consumeResult);
                                        }
                                    }
                                    _logger.LogError("Request validation error");
                                    throw new RequestValidationException("Invalid request");
                                }
                                catch (Exception e)
                                {
                                    if(e is MyKafkaException)
                                    {
                                        _logger.LogError(e,"Error sending message");
                                        throw;
                                    }
                                     _ = await base.Produce(_reviewResponseTopic, new Message<string, string>()
                                    {
                                        Key = consumeResult.Message.Key,
                                        Value = JsonConvert.SerializeObject(new MessageResponse(){ Message = e.Message}),
                                        Headers = [
                                            new Header("method", Encoding.UTF8.GetBytes("rateReview")),
                                            new Header("sender", Encoding.UTF8.GetBytes("entertaimentService")),
                                            new Header("error", Encoding.UTF8.GetBytes(e.Message))
                                        ]
                                    });
                                    _consumer.Commit(consumeResult);
                                    _logger.LogError(e, "Error sending message");
                                }
                                break;
                            default: 
                                _consumer.Commit(consumeResult);
                                
                                throw new ConsumerRecievedMessageInvalidException("Invalid message received");
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                if(_consumer != null)
                { 
                    _consumer.Dispose();
                }
                if (ex is MyKafkaException)
                {
                    _logger.LogError(ex,"Consumer error");
                    throw new ConsumerException("Consumer error ",ex);
                }
                else
                {
                    _logger.LogError(ex,"Unhandled error");
                    throw;
                }
            }
        }
    }
}