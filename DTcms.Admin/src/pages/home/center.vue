<template>
	<div class="mainbody">
		<dt-location :data="[{title:'首页'}]"></dt-location>
		<div class="content-box">
			<el-row :gutter="20">
				<el-col :span="16" :xs="24">
					<el-row :gutter="20">
						<el-col :xs="24" :md="10">
							<!--统计信息-->
							<el-row :gutter="20" class="count-box">
								<dt-site-count title="站点数量" unit="个" uri="/admin/site/view/count" />
								<dt-site-count title="会员数量" unit="位" uri="/admin/member/view/count?status=0" />
								<dt-site-count title="今日充值" unit="条" :uri="`/admin/member/recharge/view/count?status=1&startTime=${getToday()}`" />
								<dt-site-count title="今日收入" unit="元" :uri="`/admin/member/recharge/view/amount?status=1&startTime=${getToday()}`" />
							</el-row>
							<!--/统计信息-->
						</el-col>
						<el-col :xs="24" :md="14" class="mab-20">
							<!--待办事件-->
							<el-card>
								<template #header>
									<div class="card-header clearfix">
										<el-icon><Timer /></el-icon>
										<span>待办事件</span>
									</div>
								</template>
								<div class="ibtn-box">
									<el-row :gutter="20">
										<dt-site-need title="待审会员" icon="User" link="/member/audit" uri="/admin/member/view/count?status=2" />
										<dt-site-need title="待审文章" icon="Memo" uri="/admin/article/view/count?status=1" />
										<dt-site-need title="待审搞件" icon="ChatDotSquare" uri="/admin/article/contribute/view/count?status=0" />
										<dt-site-need title="待支付充值" icon="CreditCard" link="/member/recharge/list" uri="/admin/member/recharge/view/count?status=0" />
									</el-row>
								</div>
							</el-card>
							<!--/待办事件-->
						</el-col>
						<dt-user-chart type="bar" />
					</el-row>
				</el-col>
				<el-col :span="8" :xs="24">
					<el-row>
						<dt-config :user="props.user" :config="props.config" />
					</el-row>
				</el-col>
			</el-row>
		</div>
	</div>
</template>

<script setup>
	import { reactive,onMounted,getCurrentInstance } from "vue"
	import DtConfig from "../../components/certer/DtConfig.vue"
	import DtSiteCount from "../../components/certer/DtSiteCount.vue"
	import DtSiteNeed from "../../components/certer/DtSiteNeed.vue"
	import DtUserChart from "../../components/certer/DtUserChart.vue"
	
	const { proxy } = getCurrentInstance()
	
	//接收props传值
	const props = defineProps({
		config: {
			type: Object,
			default: {}
		},
		user: {
			type: Object,
			default: {}
		}
	})
	
	//获取今日日期
	const getToday = () => {
		let day = new Date()
		return `${day.getFullYear()}-${day.getMonth()}-${day.getDate()}`
	}
	
</script>

<style lang="scss">
	.count-box {
		&>:nth-child(1),
		&>:nth-child(4) {
			.el-card__body {
				background: #409eff;
				h1 {
					color: #fff;
					span {
						color: #fff;
					}
				}
			}
		}
		.el-col {
			margin-bottom: 20px;
		}
		.el-card__body {
			h1 {
				padding: 13px 0;
				text-align: center;
				color: #303133;
				font-size: 24px;
				font-weight: normal;
				white-space: nowrap;
				text-overflow: ellipsis;
				overflow: hidden;
				span {
					margin-left: 5px;
					color: #999;
					font-size: 12px;
				}
			}
			p {
				padding-bottom: 10px;
				text-align: center;
				color: #d6d3e6;
				font-size: 12px;
			}
		}
	} 
	.ibtn-box {
		margin-top: -20px;
		.el-col {
			margin-top: 20px;
		}
		.el-badge {
			width: 100%;
			a {
				display: block;
				padding: 10px;
				text-decoration: none;
				border-radius: 4px;
				text-align: center;
				background: #E6A23C;
				cursor: pointer;
				-webkit-transition-duration: 0.4s;
				transition-duration: 0.4s;
				box-sizing: border-box;
				height: 73px;
				&:hover {
					background: #409eff;
					
				}
				i {
					color: #fff;
					font-size: 28px;
				}
				span {
					display: block;
					margin-top: 5px;
					color: #fff;
					font-size: 12px;
					line-height: 20px;
				}
			}
		}
	}
	.cinfo-box {
		.el-divider {
			margin: 0 0 20px 0;
		}
		.dl-box {
			dl {
				clear: both;
				padding: 4px 0;
				height: 24px;
				line-height: 24px;
				font-size: 13px;
				dt {
					display: block;
					float: left;
					width: 60px;
				}
				dd {
					margin-left: 70px;
					color: #909399;
					white-space: nowrap;
					text-overflow: ellipsis;
					overflow: hidden;
				}
			}
		}
	}
	.store-box {
		dl {
			display: flex;
			flex-flow: row nowrap;
			justify-content: space-between;
			align-items: center;
			padding: 5px 0;
			height: 26px;
			line-height: 26px;
			color: #606266;
			dt {
				display: flex;
				flex-flow: row nowrap;
				align-items: center;
				white-space: nowrap;
				text-overflow: ellipsis;
				overflow: hidden;
				.number {
					flex-shrink: 0;
					display: block;
					margin-right: 5px;
					color: #fff;
					font-size: 12px;
					font-weight: 600;
					border-radius: 2px;
					background: #E6A23C;
					width: 20px;
					height: 20px;
					line-height: 20px;
					text-align: center;
				}
				.title {
					flex-grow: 1;
					color: #909399;
					white-space: nowrap;
					text-overflow: ellipsis;
					overflow: hidden;
				}
			}
			dd {
				flex-shrink: 0;
				margin-left: 5px;
				.count {
					color: #888;
					font-weight: 600;
					white-space: nowrap;
				}
			}
		}
	}
</style>